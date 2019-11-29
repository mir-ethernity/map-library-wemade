using System;
using System.IO;
using Xunit;

namespace Mir.Ethernity.MapLibrary.Wemade.Tests
{
    public class WemadeMapReaderTests
    {
        [Fact]
        public void LoadBichonMapTest()
        {
            var buffer = File.ReadAllBytes("./Resources/0.map");
            using(var fs = new MemoryStream(buffer))
            {
                var mapReader = new WemadeMapReader();
                var map = mapReader.Read(fs);

                Assert.NotNull(map);
                Assert.Equal(350, map.Width);
                Assert.Equal(350, map.Height);
            }
        }
    }
}
