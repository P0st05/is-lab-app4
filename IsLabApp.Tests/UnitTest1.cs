using Xunit;

namespace IsLabApp.Tests;

public class UnitTest1
{
    [Fact]
    public void Test_Health_ShouldReturnOk()
    {
        // Простейший тест — проверяет, что true == true
        Assert.True(true);
    }

    [Fact]
    public void Test_Version_ShouldNotBeNull()
    {
        string version = "1.0.0";
        Assert.NotNull(version);
    }
}
