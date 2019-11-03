using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace WCF_Chat
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceChat : IServiceChat
    {
        List<ServerUser> users = new List<ServerUser>();
        int nextID = default;

        public int Connect(string name)
        {
            ServerUser user = new ServerUser()
            {
                ID = nextID,
                Name = name,
                operationContext = OperationContext.Current
            };
            nextID++;

            SendMessage($": <{user.Name}> connected to chat.", 0);
            users.Add(user);

            return user.ID;
        }

        public void Disconnect(int ID)
        {
            var user = users.FirstOrDefault(i => i.ID == ID);
            if(user != null)
            {
                users.Remove(user);
                SendMessage($": <{user.Name}> left the chat.", 0);
            }
        }

        public void SendMessage(string message, int ID)
        {
            foreach(var item in users)
            {
                string answer = DateTime.Now.ToShortTimeString();
                var user = users.FirstOrDefault(i => i.ID == ID);
                if (user != null)
                {
                    answer += $": <{user.Name}> ";
                }

                answer += message;

                item.operationContext.GetCallbackChannel<IServerChatCallback>().MessageCallback(answer);
            }
        }
    }
}
