using Backend;
using Backend.Options;
using Backend.Pages;
using Backend.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Schema;
using Test.Mocks;
using Xunit.Abstractions;

namespace Test.Services;

public class ContainerServiceTests
{
    private readonly MockApplicationDbContext _context;
    private readonly ContainerService _containerService;
    private readonly ITestOutputHelper _output;

    public ContainerServiceTests(ITestOutputHelper output)
    {
        _output = output;

        var scb = new ServiceCollection();
        scb.AddLogging(_ => _.AddDebug());
        scb.Configure<YardOptions>(_ =>
        {
            _.BlockAmount = 4;
            _.StacksPerBlock = 5;
            _.BaysPerBlock = 5;
            _.TiersPerBlock = 5;
        });
        scb.AddSingleton<IApplicationDbContext, MockApplicationDbContext>();
        scb.AddSingleton<ContainerService>();

        var serviceCollection = scb.BuildServiceProvider();

        _context = (MockApplicationDbContext)serviceCollection.GetService<IApplicationDbContext>()!;
        _containerService = serviceCollection.GetService<ContainerService>()!;
    }


    [Theory]
    [InlineData(0, 1, 1, 1, false)]
    [InlineData(1, 0, 1, 1, false)]
    [InlineData(1, 1, 0, 1, false)]
    [InlineData(1, 1, 1, 0, false)]
    [InlineData(1, 1, 1, 1, true)]
    [InlineData(1, 2, 4, 1, true)]
    [InlineData(1, 5, 5, 1, true)]
    [InlineData(1, 1, 1, 2, false)]
    [InlineData(1, 2, 4, 3, false)]
    [InlineData(1, 5, 5, 4, false)]
    [InlineData(1, 6, 1, 1, false)]
    [InlineData(1, 1, 6, 1, false)]
    [InlineData(1, 1, 1, 6, false)]
    [InlineData(2, 1, 1, 1, true)]
    [InlineData(2, 2, 4, 1, true)]
    [InlineData(2, 5, 5, 1, true)]
    [InlineData(2, 1, 1, 2, false)]
    [InlineData(2, 2, 4, 3, false)]
    [InlineData(2, 5, 5, 4, false)]
    [InlineData(3, 1, 1, 1, true)]
    [InlineData(3, 2, 4, 1, true)]
    [InlineData(3, 5, 5, 1, true)]
    [InlineData(3, 1, 1, 2, false)]
    [InlineData(3, 2, 4, 3, false)]
    [InlineData(3, 5, 5, 4, false)]
    [InlineData(4, 1, 1, 1, true)]
    [InlineData(4, 2, 4, 1, true)]
    [InlineData(4, 5, 5, 1, true)]
    [InlineData(4, 1, 1, 2, false)]
    [InlineData(4, 2, 4, 3, false)]
    [InlineData(4, 5, 5, 4, false)]
    [InlineData(5, 1, 1, 1, false)]
    public void BasicPlacementTest(int blockId, int bayNum, int stackNum, int tierNum, bool isValid)
    {
        var container = new ContainerSchema
        {
            Id = "TestId",
            ArrivedAt = DateTime.Now,
            BlockId = blockId,
            BayNum = bayNum,
            StackNum = stackNum,
            TierNum = tierNum,
        };

        var containers = new List<ContainerSchema>() { container };

        var validation = _containerService.ValidateContainers(containers);

        Assert.Contains(container.Id, validation.Keys);
        Assert.Equal(isValid, validation[container.Id]);
    }

    [Theory]
    [InlineData(1, 1, true)]
    [InlineData(1, 2, true)]
    [InlineData(1, 3, true)]
    [InlineData(1, 4, true)]
    [InlineData(1, 5, true)]
    [InlineData(2, 5, false)]
    [InlineData(3, 5, false)]
    [InlineData(4, 5, false)]
    [InlineData(5, 5, false)]
    [InlineData(2, 4, false)]
    [InlineData(3, 4, false)]
    public void PlacementGravitySupportTest(int startTier, int endTier, bool isValid)
    {
        var containers = new List<ContainerSchema>();

        for (int tier = startTier; tier <= endTier; tier++)
        {
            containers.Add(new()
            {
                Id = "TestContainerTier-" + tier,
                BlockId = 1,
                BayNum = 1,
                StackNum = 1,
                TierNum = tier,
                ArrivedAt = DateTime.Now,
            });
        }

        var validation = _containerService.ValidateContainers(containers);

        foreach (var container in containers)
        {
            Assert.Contains(container.Id, validation.Keys);
            Assert.Equal(isValid, validation[container.Id]);
        }
    }

    public static List<object[]> ImportTestGenerator()
    {
        object[] basicImport;
        {
            basicImport =
            [
                "Basic import test",
                new List<ContainerSchema>
                {
                    new()
                    {
                        Id = "Existing1",
                        BlockId = 1,
                        BayNum = 2,
                        StackNum = 2,
                        TierNum = 1,
                        ArrivedAt = DateTime.Now,
                    }
                },
                new List<ContainerSchema>
                {
                    new()
                    {
                        Id = "New1",
                        BlockId = 1,
                        BayNum = 1,
                        StackNum = 1,
                        TierNum = 1,
                        ArrivedAt = DateTime.Now,
                    },
                    new()
                    {
                        Id = "New2",
                        BlockId = 1,
                        BayNum = 2,
                        StackNum = 2,
                        TierNum = 2,
                        ArrivedAt = DateTime.Now,
                    }
                },
                new List<string>
                {

                }
            ];
        }

        object[] collisionImport;
        {
            collisionImport =
            [
                "Collision test",
                new List<ContainerSchema>
                {
                    new()
                    {
                        Id = "Existing1",
                        BlockId = 1,
                        BayNum = 2,
                        StackNum = 2,
                        TierNum = 1,
                        ArrivedAt = DateTime.Now,
                    }
                },
                new List<ContainerSchema>
                {
                    new()
                    {
                        Id = "New1",
                        BlockId = 1,
                        BayNum = 2,
                        StackNum = 2,
                        TierNum = 1,
                        ArrivedAt = DateTime.Now,
                    },
                },
                new List<string>
                {
                    "New1"
                }
            ];
        }

        object[] idCollisionImport;
        {
            idCollisionImport =
            [
                "Id collision test",
                new List<ContainerSchema>
                {
                    new()
                    {
                        Id = "Existing1",
                        BlockId = 1,
                        BayNum = 2,
                        StackNum = 2,
                        TierNum = 1,
                        ArrivedAt = DateTime.Now,
                    }
                },
                new List<ContainerSchema>
                {
                    new()
                    {
                        Id = "Existing1",
                        BlockId = 1,
                        BayNum = 1,
                        StackNum = 1,
                        TierNum = 1,
                        ArrivedAt = DateTime.Now,
                    },
                    new()
                    {
                        Id = "Existing1",
                        BlockId = 1,
                        BayNum = 1,
                        StackNum = 1,
                        TierNum = 1,
                        ArrivedAt = DateTime.Now,
                    },
                },
                new List<string>
                {
                    "Existing1"
                }
            ];
        }

        object[] partialCollisionImport;
        {
            partialCollisionImport =
            [
                "Partial collision test",
                new List<ContainerSchema>
                {
                    new()
                    {
                        Id = "Existing1",
                        BlockId = 1,
                        BayNum = 2,
                        StackNum = 2,
                        TierNum = 1,
                        ArrivedAt = DateTime.Now,
                    }
                },
                new List<ContainerSchema>
                {
                    new()
                    {
                        Id = "New1",
                        BlockId = 1,
                        BayNum = 2,
                        StackNum = 2,
                        TierNum = 1,
                        ArrivedAt = DateTime.Now,
                    },
                    new()
                    {
                        Id = "New2",
                        BlockId = 1,
                        BayNum = 2,
                        StackNum = 2,
                        TierNum = 2,
                        ArrivedAt = DateTime.Now,
                    },
                },
                new List<string>
                {
                    "New1"
                }
            ];
        }

        return new List<object[]>
        {
            basicImport,
            collisionImport,
            idCollisionImport,
            partialCollisionImport,
        };
    }

    [Theory]
    [MemberData(nameof(ImportTestGenerator))]
    public async Task TestImport(
        string testName,
        List<ContainerSchema> existing, 
        List<ContainerSchema> containersToImport,
        List<string> faileds)
    {
        _output.WriteLine(testName);

        _context.Containers.Clear();
        _context.Containers.AddRange(existing);

        var result = await _containerService.ImportContainers(containersToImport);

        Assert.Equal(result.Success + existing.Count, _context.Containers.Count);

        foreach (var failed in faileds)
        {
            Assert.Contains(failed, result.IncorrectPositions);
        }
    }
}
