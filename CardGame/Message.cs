using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CardGame {
    public class Message {
        public string[] Lines;
        public string Label;

        private Message(string[] lines, string label) {
            Lines = lines;
            Label = label;
        }

        public static List<Message> GetAllMessages() {
            List<Message> messages = new List<Message>();
            XmlDocument doc = new XmlDocument();
            doc.Load("Data\\Messages.xml");

            foreach (XmlNode node in doc.LastChild.ChildNodes) {
                if (node.Name.ToLower() == "message") messages.Add(new Message(new string[] { node.InnerText }, node.Attributes["label"].Value));
                else if (node.Name.ToLower() == "group") {
                    List<string> texts = new List<string>();
                    foreach (XmlNode innerNode in node.ChildNodes) texts.Add(innerNode.InnerText);
                    messages.Add(new Message(texts.ToArray(), node.Attributes["label"].Value));
                }
            }

            return messages;
        }

        public static Message GetMessage(string label) {
            foreach (Message m in GetAllMessages()) if (m.Label.ToLower() == label.ToLower()) return m;
            return null;
        }
    }
}
