using System;
using System.IO;

namespace Mir.Ethernity.MapLibrary.Wemade
{
    public class WemadeMapReader : IMapReader
    {
        public Map Read(Stream stream)
        {
            var map = new Map();

            using (var reader = new BinaryReader(stream))
            {
                var unknown0 = reader.ReadBytes(22);
                map.Width = reader.ReadUInt16();
                map.Height = reader.ReadUInt16();
                var unknown1 = reader.ReadUInt16();
                map.Cells = new MapCell[map.Width, map.Height];

                for (var x = 0; x < map.Width; x++)
                {
                    for (var y = 0; y < map.Height; y++)
                    {
                        map.Cells[x, y] = new MapCell();

                    }
                }

                for (int x = 0; x < map.Width / 2; x++)
                {
                    for (int y = 0; y < map.Height / 2; y++)
                    {
                        map.Cells[(x * 2), (y * 2)].BackFile = reader.ReadByte();
                        map.Cells[(x * 2), (y * 2)].BackImage = reader.ReadUInt16();
                    }
                }

                for (int x = 0; x < map.Width; x++)
                {
                    for (int y = 0; y < map.Height; y++)
                    {
                        byte flag = reader.ReadByte();
                        map.Cells[x, y].MiddleAnimationFrame = reader.ReadByte();

                        byte value = reader.ReadByte();
                        map.Cells[x, y].FrontAnimationFrame = value == 255 ? (byte)0 : value;
                        map.Cells[x, y].FrontAnimationFrame &= 0x8F; //Probably a Blend Flag

                        map.Cells[x, y].FrontFile = reader.ReadByte();
                        map.Cells[x, y].MiddleFile = reader.ReadByte();

                        map.Cells[x, y].MiddleImage = (ushort)(reader.ReadUInt16() + 1);
                        map.Cells[x, y].FrontImage = (ushort)(reader.ReadUInt16() + 1);

                        stream.Seek(3, SeekOrigin.Current);

                        map.Cells[x, y].Light = (byte)((reader.ReadByte() & 0x0F) * 2);

                        stream.Seek(1, SeekOrigin.Current);

                        map.Cells[x, y].Flag = ((flag & 0x01) != 1) || ((flag & 0x02) != 2);
                    }
                }
            }

            return map;
        }
    }
}
