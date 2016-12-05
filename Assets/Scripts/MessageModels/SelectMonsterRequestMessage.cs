﻿using UnityEngine.Networking;

namespace Assets.Scripts.MessageModels
{
    public class SelectMonsterRequestMessage : MessageBase
    {
        public int ActionNumber;
        public int SubActionNumber;
        public string Message;
        public short MessageTypeId;
        public int SelectedMonsterTypeId;

        public SelectMonsterRequestMessage()
        {
            MessageTypeId = CustomMessageTypes.SelectMonsterRequest;
        }
    }
}
