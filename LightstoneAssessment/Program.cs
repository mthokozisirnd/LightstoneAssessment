// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Xunit;


// C# Contract for String Manipulation Service
public interface IStringManipulationService
{
    string ReverseByWords(string s);
}

// C# String Manipulation Service
public class StringManipulationService : IStringManipulationService
{
    private readonly ILogger<StringManipulationService> _logger;
    public StringManipulationService(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<StringManipulationService>();
    }

    public string ReverseByWords(string s)
    {
        String[] words = s.Split(' ');
        int left = 0, right = words.Length - 1;
        while (left <= right)
        {
            String temp = words[left];
            words[left] = words[right];
            words[right] = temp;
            left += 1;
            right -= 1;
        }
        String ans = String.Join(" ", words);

        _logger.LogInformation($"The reversed string: {ans}");
        return ans;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        //setup our DI
        var serviceProvider = new ServiceCollection()
            .AddLogging(c => c.AddConsole(opt => opt.LogToStandardErrorThreshold = LogLevel.Debug))
            .AddSingleton<IStringManipulationService, StringManipulationService>()
            .BuildServiceProvider();

        //configure console logging

        var logger = serviceProvider.GetService<ILoggerFactory>()
            .CreateLogger<Program>();
        logger.LogDebug("Starting application");

        //string manipulation

        // Declare variables and set to empty.
        string stringInput1 = "";

        // Ask the user to type their string.
        Console.Write("Type your string, and then press Enter: ");
        stringInput1 = Console.ReadLine();

        try
        {
            var stringManipulator = serviceProvider.GetService<IStringManipulationService>();
            var reversedString = stringManipulator?.ReverseByWords(stringInput1);
            logger.LogInformation(reversedString);
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, ex.Message);
        }
        
        logger.LogDebug("All done!");

    }
}

public class StringManipulationUnitTest
{   
    //Arrange
    private readonly IStringManipulationService _service;
    public StringManipulationUnitTest(IStringManipulationService service)
    {
        _service = service; 
    }

    [Fact]
    [InlineData("This is the day")]
    public void PassingReverseByWords(string s)
    {

        //Action

        var reversedString = _service.ReverseByWords(s);

        //Assert

        Assert.Equal(reversedString, "day the is This");

    }

    [Fact]
    [InlineData("This is the day")]
    public void FailingReverseByWords(string s)
    {
        //Action
        var reversedString = _service.ReverseByWords(s);

        //Assert

        Assert.Equal(s, reversedString);
    }

    [Fact]
    [InlineData("This is the day")]
    public void Add_EmptyString_ReturnsEmptyString(string s)
    {
        //Action

        var reversedString = _service.ReverseByWords(s);

        //Assert

        Assert.Equal(string.Empty, reversedString);
    }
}

