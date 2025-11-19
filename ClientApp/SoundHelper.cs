using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApp
{
    public static class SoundHelper
    {
        // Import Windows API để phát âm thanh MP3
        [DllImport("winmm.dll")]
        private static extern int mciSendString(string command, string buffer, int bufferSize, IntPtr hWndCallback);

        // Lưu danh sách các alias đang mở
        private static List<string> _openAliases = new List<string>();

        // Import Windows API để phát âm thanh
        [DllImport("kernel32.dll")]
        private static extern bool Beep(int frequency, int duration);

        [DllImport("user32.dll")]
        private static extern bool MessageBeep(uint uType);

        private const uint MB_OK = 0x00000000;
        private const uint MB_ICONASTERISK = 0x00000040;
        private const uint MB_ICONEXCLAMATION = 0x00000030;
        private const uint MB_ICONHAND = 0x00000010;

        // Đường dẫn đến thư mục Sounds
        private static string GetSoundsFolder()
        {
            string soundsPath = Path.Combine(Application.StartupPath, "Sounds");
            if (!Directory.Exists(soundsPath))
            {
                Directory.CreateDirectory(soundsPath);
                
                // Tự động tạo file WAV nếu chưa có file nào
                try
                {
                    var files = Directory.GetFiles(soundsPath, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(f => f.EndsWith(".wav", StringComparison.OrdinalIgnoreCase) || 
                                    f.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    
                    if (files.Count == 0)
                    {
                        WavGenerator.GenerateAllSoundFiles(soundsPath);
                        Console.WriteLine("[Sound] ✅ Đã tự động tạo file âm thanh WAV");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Sound] ⚠️ Không thể tạo file WAV tự động: {ex.Message}");
                }
            }
            return soundsPath;
        }

        // Tìm file âm thanh theo pattern trong tên file
        private static string FindSoundFile(string[] patterns, string[] extensions = null)
        {
            if (extensions == null)
                extensions = new[] { ".wav", ".mp3", ".WAV", ".MP3" };

            string soundsFolder = GetSoundsFolder();
            var allFiles = Directory.GetFiles(soundsFolder, "*.*", SearchOption.TopDirectoryOnly)
                .Where(f => extensions.Any(ext => f.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            // Tìm file theo pattern
            foreach (var pattern in patterns)
            {
                var found = allFiles.FirstOrDefault(f => 
                    Path.GetFileNameWithoutExtension(f).IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0);
                if (found != null)
                {
                    Console.WriteLine($"[Sound] ✅ Tìm thấy file: {Path.GetFileName(found)} (pattern: {pattern})");
                    return found;
                }
            }

            return null;
        }

        // Phát file WAV - Sử dụng SoundPlayer (đơn giản và đáng tin cậy)
        private static void PlayWavFile(string fileName)
        {
            try
            {
                string filePath = Path.Combine(GetSoundsFolder(), fileName);
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"[Sound] ❌ File WAV không tồn tại: {filePath}");
                    return;
                }

                // Sử dụng SoundPlayer - đơn giản và hoạt động tốt
                using (System.Media.SoundPlayer player = new System.Media.SoundPlayer(filePath))
                {
                    player.Play(); // Phát không đồng bộ
                    Console.WriteLine($"[Sound] ✅ Đã phát file WAV: {fileName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Sound] ❌ Lỗi phát WAV: {ex.Message}");
                throw;
            }
        }

        // Phát file MP3 - Sử dụng nhiều phương pháp để đảm bảo hoạt động
        private static void PlayMp3File(string fileName)
        {
            try
            {
                string filePath = Path.Combine(GetSoundsFolder(), fileName);
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"[Sound] ❌ File không tồn tại: {filePath}");
                    Console.WriteLine($"[Sound] 💡 Vui lòng copy file {fileName} vào thư mục: {GetSoundsFolder()}");
                    throw new FileNotFoundException($"File không tồn tại: {filePath}");
                }

                Console.WriteLine($"[Sound] 🎵 Đang phát file: {fileName}");

                // Phương pháp 1: Sử dụng Windows Media Player COM (đáng tin cậy nhất)
                try
                {
                    Type wmpType = Type.GetTypeFromProgID("WMPlayer.OCX");
                    if (wmpType != null)
                    {
                        object wmpObject = Activator.CreateInstance(wmpType);
                        var wmp = wmpObject.GetType();
                        var media = wmp.InvokeMember("newMedia", System.Reflection.BindingFlags.InvokeMethod, null, wmpObject, new object[] { filePath });
                        var controls = wmp.InvokeMember("controls", System.Reflection.BindingFlags.GetProperty, null, wmpObject, null);
                        controls.GetType().InvokeMember("play", System.Reflection.BindingFlags.InvokeMethod, null, controls, null);
                        
                        // Đợi một chút rồi dừng và giải phóng
                        Task.Run(async () =>
                        {
                            await Task.Delay(3000);
                            try
                            {
                                controls.GetType().InvokeMember("stop", System.Reflection.BindingFlags.InvokeMethod, null, controls, null);
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(wmpObject);
                            }
                            catch { }
                        });
                        
                        Console.WriteLine($"[Sound] ✅ Đã phát file bằng WMP COM: {fileName}");
                        return;
                    }
                }
                catch (Exception ex1)
                {
                    Console.WriteLine($"[Sound] WMP COM failed: {ex1.Message}");
                }

                // Phương pháp 2: Sử dụng wmplayer.exe (Windows Media Player)
                try
                {
                    string wmplayerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Windows Media Player", "wmplayer.exe");
                    if (!File.Exists(wmplayerPath))
                    {
                        wmplayerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Windows Media Player", "wmplayer.exe");
                    }
                    
                    if (File.Exists(wmplayerPath))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = wmplayerPath,
                            Arguments = $"/play /close \"{filePath}\"",
                            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                            CreateNoWindow = true
                        });
                        Console.WriteLine($"[Sound] ✅ Đã phát file bằng wmplayer.exe: {fileName}");
                        return;
                    }
                }
                catch (Exception ex2)
                {
                    Console.WriteLine($"[Sound] wmplayer.exe failed: {ex2.Message}");
                }

                // Phương pháp 3: Sử dụng mciSendString
                try
                {
                    StopMp3();
                    string alias = $"mp3_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
                    string command = $"open \"{filePath}\" type mpegvideo alias {alias}";
                    int result = mciSendString(command, null, 0, IntPtr.Zero);
                    
                    if (result == 0)
                    {
                        lock (_openAliases)
                        {
                            _openAliases.Add(alias);
                        }
                        
                        command = $"play {alias} from 0";
                        result = mciSendString(command, null, 0, IntPtr.Zero);
                        
                        if (result == 0)
                        {
                            Console.WriteLine($"[Sound] ✅ Đã phát file bằng MCI: {fileName}");
                            
                            Task.Run(async () =>
                            {
                                await Task.Delay(3000);
                                try
                                {
                                    mciSendString($"close {alias}", null, 0, IntPtr.Zero);
                                    lock (_openAliases)
                                    {
                                        _openAliases.Remove(alias);
                                    }
                                }
                                catch { }
                            });
                            return;
                        }
                        else
                        {
                            mciSendString($"close {alias}", null, 0, IntPtr.Zero);
                        }
                    }
                }
                catch (Exception ex3)
                {
                    Console.WriteLine($"[Sound] MCI failed: {ex3.Message}");
                }

                // Phương pháp 4: Sử dụng Process.Start với file trực tiếp
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true,
                        WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                    });
                    Console.WriteLine($"[Sound] ✅ Đã phát file bằng Process.Start: {fileName}");
                }
                catch (Exception ex4)
                {
                    Console.WriteLine($"[Sound] Process.Start failed: {ex4.Message}");
                    throw new Exception("Tất cả phương pháp phát MP3 đều thất bại");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Sound] ❌ Lỗi phát MP3: {ex.Message}");
                throw;
            }
        }

        // Dừng và đóng tất cả file MP3 đang phát
        private static void StopMp3()
        {
            try
            {
                lock (_openAliases)
                {
                    foreach (var alias in _openAliases.ToArray())
                    {
                        try
                        {
                            mciSendString($"stop {alias}", null, 0, IntPtr.Zero);
                            mciSendString($"close {alias}", null, 0, IntPtr.Zero);
                        }
                        catch { }
                    }
                    _openAliases.Clear();
                }
            }
            catch { }
        }

        // Phát âm thanh khi viết tin nhắn (gõ phím)
        public static void PlayTypingSound()
        {
            try
            {
                // Tìm file âm thanh typing (ưu tiên các pattern)
                string[] patterns = { "typing", "type", "keyboard", "click" };
                string foundFile = FindSoundFile(patterns);
                
                if (foundFile != null)
                {
                    string fileName = Path.GetFileName(foundFile);
                    if (foundFile.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
                    {
                        Task.Run(() => PlayWavFile(fileName));
                        return;
                    }
                    else if (foundFile.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                    {
                        Task.Run(() =>
                        {
                            try
                            {
                                PlayMp3File(fileName);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[Sound] MP3 failed: {ex.Message}");
                            }
                        });
                        return;
                    }
                }

                // Fallback: phát beep ngắn
                try
                {
                    Beep(800, 50);
                    Console.WriteLine("[Sound] ✅ PlayTypingSound: Beep success (no sound file found)");
                }
                catch
                {
                    try
                    {
                        Console.Beep(800, 50);
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine($"[Sound] ❌ All typing sound methods failed: {ex2.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Sound] Error in PlayTypingSound: {ex.Message}");
                // Vẫn thử phát beep
                try { Beep(800, 50); } catch { }
            }
        }

        // Phát âm thanh khi gửi tin nhắn
        public static void PlaySendSound()
        {
            try
            {
                // Tìm file âm thanh send (ưu tiên các pattern)
                string[] patterns = { "send", "sent", "ting", "ting_clean", "success", "confirm" };
                string foundFile = FindSoundFile(patterns);
                
                if (foundFile != null)
                {
                    string fileName = Path.GetFileName(foundFile);
                    if (foundFile.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
                    {
                        Task.Run(() => PlayWavFile(fileName));
                        return;
                    }
                    else if (foundFile.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                    {
                        Task.Run(() =>
                        {
                            try
                            {
                                PlayMp3File(fileName);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[Sound] MP3 failed: {ex.Message}");
                            }
                        });
                        return;
                    }
                }

                // Fallback: thử các phương pháp khác
                try
                {
                    Beep(1000, 150);
                    Console.WriteLine("[Sound] ✅ PlaySendSound: Beep API success (no sound file found)");
                }
                catch
                {
                    try
                    {
                        Console.Beep(1000, 150);
                        Console.WriteLine("[Sound] ✅ PlaySendSound: Console.Beep success");
                    }
                    catch
                    {
                        try
                        {
                            MessageBeep(MB_ICONASTERISK);
                            Console.WriteLine("[Sound] ✅ PlaySendSound: MessageBeep success");
                        }
                        catch
                        {
                            try
                            {
                                SystemSounds.Asterisk.Play();
                                Console.WriteLine("[Sound] ✅ PlaySendSound: SystemSounds success");
                            }
                            catch (Exception ex2)
                            {
                                Console.WriteLine($"[Sound] ❌ All send sound methods failed: {ex2.Message}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Sound] Error playing send sound: {ex.Message}");
                // Vẫn thử phát beep
                try { Beep(1000, 150); } catch { }
            }
        }

        // Phát âm thanh khi nhận tin nhắn
        public static void PlayReceiveSound()
        {
            try
            {
                // Tìm file âm thanh receive (ưu tiên các pattern)
                string[] patterns = { "receive", "received", "message_receive", "incoming", "notification", "alert" };
                string foundFile = FindSoundFile(patterns);
                
                if (foundFile != null)
                {
                    string fileName = Path.GetFileName(foundFile);
                    if (foundFile.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
                    {
                        Task.Run(() => PlayWavFile(fileName));
                        return;
                    }
                    else if (foundFile.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
                    {
                        Task.Run(() =>
                        {
                            try
                            {
                                PlayMp3File(fileName);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[Sound] MP3 failed: {ex.Message}");
                            }
                        });
                        return;
                    }
                }

                // Fallback: thử các phương pháp khác
                try
                {
                    Beep(800, 200);
                    System.Threading.Thread.Sleep(50);
                    Beep(1000, 150);
                    Console.WriteLine("[Sound] ✅ PlayReceiveSound: Beep API success (no sound file found)");
                }
                catch
                {
                    try
                    {
                        Console.Beep(800, 200);
                        System.Threading.Thread.Sleep(50);
                        Console.Beep(1000, 150);
                        Console.WriteLine("[Sound] ✅ PlayReceiveSound: Console.Beep success");
                    }
                    catch
                    {
                        try
                        {
                            MessageBeep(MB_ICONEXCLAMATION);
                            Console.WriteLine("[Sound] ✅ PlayReceiveSound: MessageBeep success");
                        }
                        catch
                        {
                            try
                            {
                                SystemSounds.Exclamation.Play();
                                Console.WriteLine("[Sound] ✅ PlayReceiveSound: SystemSounds success");
                            }
                            catch (Exception ex2)
                            {
                                Console.WriteLine($"[Sound] ❌ All receive sound methods failed: {ex2.Message}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Sound] Error playing receive sound: {ex.Message}");
                // Vẫn thử phát beep
                try 
                { 
                    Beep(800, 200);
                    System.Threading.Thread.Sleep(50);
                    Beep(1000, 150);
                } 
                catch { }
            }
        }

        // Hàm test âm thanh - để kiểm tra xem âm thanh có hoạt động không
        public static void TestSound()
        {
            Console.WriteLine("[Sound] 🧪 Testing sound system...");
            PlaySendSound();
            System.Threading.Thread.Sleep(500);
            PlayReceiveSound();
            System.Threading.Thread.Sleep(500);
            PlayTypingSound();
            Console.WriteLine("[Sound] ✅ Test completed");
        }
    }
}

