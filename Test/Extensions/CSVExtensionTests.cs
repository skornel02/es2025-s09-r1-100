﻿using Backend.Extensions;
using Microsoft.AspNetCore.Http;

namespace Test.Extensions;

public class CSVExtensionTests
{
    private const string CSV = """
container,block,bay,stack,tier,unixtime
WKRE1092594,4,4,4,1,1704412800000
CGXK0664256,1,1,3,1,1704412800000
UKZM2157184,3,2,4,2,1704499200000
ZUKN0754509,4,4,5,2,1704499200000
UOUO6930417,1,4,3,2,1704499200000
DTOJ9251827,2,2,4,2,1704499200000
BKZB5908110,1,2,4,2,1704499200000
YNZC8660804,4,3,2,2,1704499200000
YLRV1154262,1,5,3,2,1704585600000
QKBI2083226,1,2,2,2,1704585600000
OKJA6371722,3,3,4,2,1704585600000
YHEF9302287,2,3,2,2,1704672000000
IVFL7614580,1,1,3,2,1704672000000
SPXA8466444,3,5,3,2,1704672000000
XPTS4710353,4,4,3,2,1704672000000
HGGA7596600,3,1,3,2,1704672000000
BZXR1161550,4,3,4,2,1704672000000
RYJL3552725,2,4,2,2,1704672000000
EPAJ1621345,3,5,4,2,1704758400000
VMIO9001550,4,2,3,2,1704758400000
HSYM1074438,1,5,2,2,1704758400000
OFFS4866828,4,4,4,2,1704758400000
YOMW6691481,3,2,2,2,1704758400000
AEJW7335202,2,2,2,2,1704758400000
PHKF3732733,2,4,4,2,1704844800000
GJRO1745948,2,5,3,2,1704844800000
NWLQ8313847,1,3,4,2,1704844800000
HHXC3874483,4,4,2,2,1704844800000
UGNH3865042,4,5,4,2,1704844800000
MMGF9561504,3,5,2,2,1704844800000
CVZP1821978,4,1,4,2,1704844800000
VXIL3929698,2,3,2,3,1704931200000
REKJ6014441,4,5,4,3,1704931200000
GLIB6632508,1,1,3,3,1705017600000
LQML2133177,2,2,2,3,1705017600000
RTPU3019377,3,2,4,3,1705017600000
UPIL3269205,4,1,4,3,1705017600000
VPET0154668,2,4,4,3,1705017600000
RFGD0287927,3,1,3,3,1705017600000
IYLW0747461,4,2,3,3,1705104000000
GQVN4096789,4,4,2,1,1704240000000
MVJA5910379,3,1,3,1,1704326400000
YVOY9902889,2,3,2,1,1704499200000
JIVA8444489,1,5,3,1,1704499200000
AUMZ5365009,2,2,4,1,1704240000000
""";

    internal sealed class MockFile : IFormFile
    {
        private string _content { get; set; }

        public string ContentType => throw new NotImplementedException();

        public string ContentDisposition => throw new NotImplementedException();

        public IHeaderDictionary Headers => throw new NotImplementedException();

        public long Length => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public string FileName => throw new NotImplementedException();

        public MockFile(string content)
        {
            _content = content;
        }

        public void CopyTo(Stream target)
        {
            throw new NotImplementedException();
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Stream OpenReadStream()
        {
            // string to stream
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.WriteLine(_content);
            writer.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }
    }

    [Fact]
    public async Task ParsesExample()
    {
        var file = new MockFile(CSV);
        var parsed = await file.ParseCsvAsync();
        Assert.NotNull(parsed);
        Assert.Equal(CSV.Split("\n").Count(), parsed.Count);
        Assert.Equal("container", parsed[0][0]);
        Assert.Equal("unixtime", parsed[0][5]);
    }
}
