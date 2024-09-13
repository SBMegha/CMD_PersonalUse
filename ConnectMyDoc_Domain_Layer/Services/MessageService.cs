using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;

namespace ConnectMyDoc_Domain_Layer.Services
{
    public class MessageService : IMessageService
    {
        private readonly Dictionary<string, string> _messages = null;

        public MessageService(IConfiguration configuration)
        {
            // Load messages from XML file
            var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var xmlFilePath = Path.Combine(assemblyLocation, "Resources", "PatientCustomExceptions.xml");
            _messages = LoadMessages(xmlFilePath);

        }


        private Dictionary<string, string> LoadMessages(string xmlFilePath)
        {
            var messages = new Dictionary<string, string>();
            if (File.Exists(xmlFilePath))
            {
                var document = XDocument.Load(xmlFilePath);
                messages = document.Descendants("Message")
                                   .ToDictionary(
                                       m => m.Attribute("key")?.Value,
                                       m => m.Value
                                   );
            }
            else
            {
                // Handle the case where the file doesn't exist
                // You might want to log this error or throw an exception
                throw new FileNotFoundException("Exception messages file not found.", xmlFilePath);
            }

            return messages;

        }

        public string GetMessage(string key)
        {
            return _messages.TryGetValue(key, out var message) ? message : "Unknown error occurred.";
        }
        
    }
}
