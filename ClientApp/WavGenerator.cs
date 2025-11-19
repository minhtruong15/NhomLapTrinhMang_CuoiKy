using System;
using System.IO;

namespace ClientApp
{
    /// <summary>
    /// Class để tạo file WAV đơn giản cho âm thanh thông báo
    /// </summary>
    public static class WavGenerator
    {
        /// <summary>
        /// Tạo file WAV với tone đơn giản
        /// </summary>
        /// <param name="filePath">Đường dẫn file output</param>
        /// <param name="frequency">Tần số (Hz) - 440 = nốt A, 800 = tone cao, 500 = tone thấp</param>
        /// <param name="durationMs">Thời lượng (milliseconds)</param>
        /// <param name="sampleRate">Sample rate (44100 = CD quality)</param>
        public static void GenerateWavFile(string filePath, int frequency, int durationMs, int sampleRate = 44100)
        {
            try
            {
                int numSamples = (int)(sampleRate * durationMs / 1000.0);
                int numChannels = 1; // Mono
                int bitsPerSample = 16;
                int byteRate = sampleRate * numChannels * bitsPerSample / 8;
                int blockAlign = numChannels * bitsPerSample / 8;
                int dataSize = numSamples * blockAlign;
                int fileSize = 36 + dataSize;

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    // WAV header
                    writer.Write("RIFF".ToCharArray());
                    writer.Write(fileSize);
                    writer.Write("WAVE".ToCharArray());
                    writer.Write("fmt ".ToCharArray());
                    writer.Write(16); // Subchunk1Size
                    writer.Write((short)1); // AudioFormat (1 = PCM)
                    writer.Write((short)numChannels);
                    writer.Write(sampleRate);
                    writer.Write(byteRate);
                    writer.Write((short)blockAlign);
                    writer.Write((short)bitsPerSample);
                    writer.Write("data".ToCharArray());
                    writer.Write(dataSize);

                    // Generate sine wave
                    double amplitude = 0.3; // Volume (0.0 to 1.0)
                    double twoPi = 2.0 * Math.PI;
                    double phaseIncrement = twoPi * frequency / sampleRate;

                    for (int i = 0; i < numSamples; i++)
                    {
                        // Sine wave với fade in/out để tránh click
                        double fade = 1.0;
                        if (i < numSamples * 0.1) // Fade in 10%
                            fade = i / (numSamples * 0.1);
                        else if (i > numSamples * 0.9) // Fade out 10%
                            fade = (numSamples - i) / (numSamples * 0.1);

                        double sample = Math.Sin(phaseIncrement * i) * amplitude * fade;
                        short sampleValue = (short)(sample * short.MaxValue);
                        writer.Write(sampleValue);
                    }
                }

                Console.WriteLine($"[WavGenerator] ✅ Đã tạo file: {filePath} ({frequency}Hz, {durationMs}ms)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WavGenerator] ❌ Lỗi tạo file WAV: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Tạo 3 file WAV cho dự án: typing, send, receive
        /// </summary>
        public static void GenerateAllSoundFiles(string soundsFolder)
        {
            try
            {
                if (!Directory.Exists(soundsFolder))
                {
                    Directory.CreateDirectory(soundsFolder);
                }

                // 1. Âm thanh khi viết tin nhắn (gõ phím) - tone cao, ngắn
                string typingFile = Path.Combine(soundsFolder, "typing-sound.wav");
                GenerateWavFile(typingFile, 800, 100, 44100);

                // 2. Âm thanh khi gửi tin nhắn - tone trung bình, vừa
                string sendFile = Path.Combine(soundsFolder, "send-sound.wav");
                GenerateWavFile(sendFile, 600, 200, 44100);

                // 3. Âm thanh khi nhận tin nhắn - tone thấp hơn, dài hơn
                string receiveFile = Path.Combine(soundsFolder, "receive-sound.wav");
                GenerateWavFile(receiveFile, 500, 300, 44100);

                Console.WriteLine($"[WavGenerator] ✅ Đã tạo tất cả 3 file âm thanh trong: {soundsFolder}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WavGenerator] ❌ Lỗi tạo file âm thanh: {ex.Message}");
                throw;
            }
        }
    }
}

