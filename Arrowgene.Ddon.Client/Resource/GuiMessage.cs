﻿using System;
using System.Collections.Generic;
using System.Text;
using Arrowgene.Buffers;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.Client.Resource
{
    public class GuiMessage : ResourceFile
    {
        private static readonly ILogger Logger = LogProvider.Logger<Logger>(typeof(GuiMessage));

        public class Entry
        {
            public uint Index { get; set; }
            public string Key { get; set; }
            public string Msg { get; set; }
            public uint a2 { get; set; }
            public uint a3 { get; set; }
            public uint a4 { get; set; }
            public uint a5 { get; set; }
            public uint KeyReadIndex { get; set; }
            public uint MsgReadIndex { get; set; }
        }

        public List<Entry> Entries { get; }
        public byte[] unk { get; set; }
        public string Str { get; set; }

        public GuiMessage()
        {
            Entries = new List<Entry>();
        }

        protected override void ReadResource(IBuffer buffer)
        {
            uint version = ReadUInt32(buffer);
            uint a = ReadUInt32(buffer);
            uint b = ReadUInt32(buffer);
            uint c = ReadUInt32(buffer);
            uint keyCount = ReadUInt32(buffer);
            uint stringCount = ReadUInt32(buffer);
            uint keySize = ReadUInt32(buffer);
            uint stringSize = ReadUInt32(buffer);
            uint strLen = ReadUInt32(buffer);
            Str = buffer.ReadString((int)strLen);
            buffer.ReadByte(); // str null-termination


            if (keyCount > 0 && keyCount != stringCount)
            {
                // TODO it seems to work for this case as well
                // This case exists for a few files, one is 
                // /Volumes/data/game/Dragon's Dogma Online/nativePC/rom/quest/pqi_01.arc
                // ui\00_message\package_quest\package_quest_info1
            }

            uint maxEntries = Math.Max(keyCount, stringCount);
            Entry[] entries = new Entry[maxEntries];
            for (int i = 0; i < maxEntries; i++)
            {
                entries[i] = new Entry();
            }

            if (keyCount > 0)
            {
                // TODO I assume this part is only parsed when "keyCount > 0"
                for (int i = 0; i < keyCount; i++)
                {
                    Entry entry = entries[i];
                    entry.Index = ReadUInt32(buffer);
                    entry.a2 = ReadUInt32(buffer);
                    entry.a3 = ReadUInt32(buffer);
                    entry.a4 = ReadUInt32(buffer);
                    entry.a5 = ReadUInt32(buffer);
                }

                unk = buffer.ReadBytes(1024); // hashTable?

                for (uint i = 0; i < keyCount; i++)
                {
                    Entry entry = entries[i];
                    entry.Key = buffer.ReadCString(Encoding.UTF8);
                    entry.KeyReadIndex = i;
                }
            }

            for (uint i = 0; i < stringCount; i++)
            {
                Entry entry = entries[i];
                entry.Msg = buffer.ReadCString(Encoding.UTF8);
                entry.MsgReadIndex = i;
            }

            Entries.Clear();
            Entries.AddRange(entries);
        }
    }
}
