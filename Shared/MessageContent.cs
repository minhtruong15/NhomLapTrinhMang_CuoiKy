using System;
using Newtonsoft.Json;

namespace Shared
{
    public class MessageContent
    {
        public bool IsImage { get; set; }
        public string Text { get; set; }
        public string FileName { get; set; }
        public string Base64Data { get; set; }
        public string ClientMessageId { get; set; }

        public static string Serialize(MessageContent content)
            => JsonConvert.SerializeObject(content);

        public static MessageContent Deserialize(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return CreateText(string.Empty);

            try
            {
                var content = JsonConvert.DeserializeObject<MessageContent>(raw);
                if (content != null)
                {
                    if (string.IsNullOrWhiteSpace(content.ClientMessageId))
                        content.ClientMessageId = Guid.NewGuid().ToString("N");
                    return content;
                }
            }
            catch
            {
                // ignored - fallback to plain text
            }

            return CreateText(raw);
        }

        public static MessageContent CreateText(string text)
        {
            return new MessageContent
            {
                IsImage = false,
                Text = text ?? string.Empty,
                ClientMessageId = Guid.NewGuid().ToString("N")
            };
        }

        public static MessageContent CreateImage(string fileName, string base64)
        {
            return new MessageContent
            {
                IsImage = true,
                FileName = fileName ?? "image",
                Base64Data = base64,
                ClientMessageId = Guid.NewGuid().ToString("N")
            };
        }
    }
}
