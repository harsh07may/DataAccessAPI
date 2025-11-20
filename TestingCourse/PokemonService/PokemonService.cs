using TestingCourse.Pokecenter;

namespace TestingCourse.PokemonService;

public class PokemonService
{
    public readonly IPokeCenter _pokeCenter;
    public PokemonService(IPokeCenter pokeCenter)
    {
        _pokeCenter = pokeCenter;
    }

    public string ReturnsPikachuIfZero(int num)
    {

        if(num == 0)
            return "PIKACHU";
        else
            return "SQUIRTLE";
        
    }

    public int CalculateSum(int a, int b)
    {
        return a + b;
    }

    public  DateTime LastModifiedDate()
    {
        return DateTime.Now;
    }

    public Pokemon GetPikachu()
    {
        var pokemon = new Pokemon()
        {
            Name = "Pikachu",
            Type = "Electric"
        };

        return pokemon;
    }

    public IEnumerable<Pokemon> GetParty()
    {
        IEnumerable<Pokemon> pokemon = new[]
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

        return pokemon;
    }

    public string HealPokemon()
    {
        var pokeCenterStatus = _pokeCenter.IsOpen();
        
        if (pokeCenterStatus)
            return "Pokemon HP was restored!";
        else
            return "Unable to restore HP.!";

    }

}
