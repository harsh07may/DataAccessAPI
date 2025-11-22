/* PLAN: Unit testing best practices
 * 
 *	Naming convention - MethodName_ScenarioUnderTest_ExpectedResults
 *	https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices#follow-test-naming-standards
 *	
 *	Arrange - Get the variables, classes etc.
 *	Act - Execute the methods
 *	Asset - Verify whatever is returns
 *	https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices#arrange-your-tests
 *	
 *	Create a globally available SUT - System under test.
 *	
 *	Mocking dependencies with NSubsstitue
 *	
 *	
 *	TODO: Write Unit tests for the actual WEB APIs next.
 */


namespace TestingCourse.PokemonService;

public class PokemonServiceTests
{
	private readonly PokemonService _sut;
    private readonly IPokeCenter _pokecenter;
    public PokemonServiceTests()
    {
        // DEPENDENCIES
        _pokecenter = Substitute.For<IPokeCenter>();

        // SUT
        _sut = new PokemonService(_pokecenter);
    }

    [Fact]
    public void ReturnsPikachuIfZero_IfZeroPassed_ReturnsString()
    {
		// Arrange
		int num = 0;

		// Act
		string result = _sut.ReturnsPikachuIfZero(num);

		// Asset
		result.Should().BeEquivalentTo("PIKACHU");
		result.Should().NotBeNullOrWhiteSpace();
    }


	[Theory]
	[InlineData(0, 0, 0)]
    [InlineData(-10, -10, -20)]
    [InlineData(10, 10, 20)]
    public void CalculateSum_IfTwoIntegers_ReturnSum(int a, int b, int sum)
	{
        // Arrange

        // Act 
        int result = _sut.CalculateSum(a, b);

		// Assert
		result.Should().Be(sum);

    }

	[Fact]
	public void LastModifiedDate_WhenCalled_ReturnValidDate()
	{
        // Arrange

        // Act
        var result = _sut.LastModifiedDate();

        // Asset
        result.Should().BeAfter(1.January(2025));
        result.Should().BeBefore(1.December(2025));
    }


    [Fact]
    public void GetPikachu_WhenCalled_ReturnsObject()
    {
        // Arrange
        Pokemon expectedPokemon = new()
        {
            Name = "Pikachu",
            Type = "Electric"
        };

        // Act
        var result = _sut.GetPikachu();

        // Assert
        result.Should().BeOfType<Pokemon>();
        result.Should().BeEquivalentTo(expectedPokemon);
        result.Name.Should().Be("Pikachu");
    }

    [Fact]
    public void GetParty_WhenCalled_ReturnsEnuerable()
    {
        // Arrange
        IEnumerable<Pokemon> expectedParty = new[]
        {
            new Pokemon
            {
                Name = "Pikachu",
                Type = "Electric"
            },
            new Pokemon
            {
                Name = "Charizard",
                Type = "Fire"
            },
            new Pokemon
            {
                Name = "Blastoise",
                Type = "Water"
            }
        };

        // Act
        var result = _sut.GetParty();

        // Assert
        result.Should().BeAssignableTo<IEnumerable<Pokemon>>();
        result.Should().BeEquivalentTo(expectedParty);
        result.Should().Contain(x => x.Name == "Charizard");
    }

    [Fact]
    public void HealPokemon_WhenCalled_ReturnsStatus()
    {
        // Arrange
        _pokecenter.IsOpen().Returns(true); 

        // Act
        var result = _sut.HealPokemon();

        // Assert
        result.Should().BeEquivalentTo("Pokemon HP was restored!");
    }
}
