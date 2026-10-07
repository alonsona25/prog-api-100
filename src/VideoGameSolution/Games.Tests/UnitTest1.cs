namespace Games.Tests;

public class UnitTest1
{
    [Fact]
    public void CanAddTwoNumbers()
    {
        // Given Arrange
        int a = 10; int b = 21; int answer;

        // When Act
        answer = a + b; // SUT
        // Then Assert
        Assert.Equal(31, answer);
    }

    [Theory]
    [InlineData(10,21,31)]
    [InlineData(4,5, 9)]
    [InlineData(10,2,12)]
    public void CanAddTheseNumbers(int a, int b, int expected)
    {
        var answer = a + b;
        Assert.Equal(expected, answer);
    }
}
