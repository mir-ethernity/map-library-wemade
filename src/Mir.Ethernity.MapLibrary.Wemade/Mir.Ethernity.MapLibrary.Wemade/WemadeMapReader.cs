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

                for (ushort x = 0; x < map.Width; x++)
                {
                    for (ushort y = 0; y < map.Height; y++)
                    {
                        map.Cells[x, y] = new MapCell();

                    }
                }

                for (ushort x = 0; x < map.Width / 2; x++)
                {
                    for (ushort y = 0; y < map.Height / 2; y++)
                    {
                        var cell = map.Cells[(x * 2), (y * 2)];
                        var backFile = reader.ReadByte();
                        cell.Back = new MapCellLayer
                        {
                            FileType = (MapFileType)(backFile % 15),
                            TileType = (MapTileType)Math.Floor(backFile / 15M),
                            ImageIndex = reader.ReadUInt16()
                        };
                    }
                }

                for (ushort x = 0; x < map.Width; x++)
                {
                    for (ushort y = 0; y < map.Height; y++)
                    {
                        MapCell cell = map.Cells[x, y];

                        byte flag = reader.ReadByte();
                        var middleAnimationFrame = reader.ReadByte();

                        var frontAnimationFrame = reader.ReadByte();

                        var frontFile = reader.ReadByte();
                        var middleFile = reader.ReadByte();
                        var middleImageIndex = reader.ReadUInt16();
                        var frontImageIndex = reader.ReadUInt16();

                        stream.Seek(3, SeekOrigin.Current);

                        cell.Light = (byte)((reader.ReadByte() & 0x0F) * 2);

                        stream.Seek(1, SeekOrigin.Current);

                        cell.Flag = ((flag & 0x01) != 1) || ((flag & 0x02) != 2);

                        if (frontFile < byte.MaxValue && frontImageIndex < ushort.MaxValue)
                        {
                            cell.Front = new MapCellLayer
                            {
                                AnimationFrame = frontAnimationFrame < byte.MaxValue ? new byte?((byte)(frontAnimationFrame & 0x8F)) : null,
                                FileType = (MapFileType)(frontFile % 15),
                                TileType = (MapTileType)Math.Floor(frontFile / 15M),
                                ImageIndex = frontImageIndex
                            };
                        }

                        if (middleFile < byte.MaxValue && middleImageIndex < ushort.MaxValue)
                        {
                            cell.Middle = new MapCellLayer
                            {
                                AnimationFrame = middleAnimationFrame < byte.MaxValue ? new byte?(middleAnimationFrame) : null,
                                FileType = (MapFileType)(middleFile % 15),
                                TileType = (MapTileType)Math.Floor(middleFile / 15M),
                                ImageIndex = middleImageIndex
                            };
                        }
                    }
                }
            }

            return map;
        }
    }
}
