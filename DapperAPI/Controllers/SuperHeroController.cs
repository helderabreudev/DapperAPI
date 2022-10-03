using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace DapperAPI.Controllers;

[Route("api/[controller]")]
public class SuperHeroController : ControllerBase
{
	private readonly IConfiguration _config;

	public SuperHeroController(IConfiguration config)
	{
		_config = config;
	}

	[HttpGet]
	public async Task<ActionResult<List<SuperHero>>> GetAllSuperHeroes()
	{
		using var connection = new SqlConnection(_config.GetConnectionString("Test"));
		IEnumerable<SuperHero> heroes = await SelectAllHeroes(connection);
		return Ok(heroes);
	}

	[HttpGet("{heroId}")]
	public async Task<ActionResult<SuperHero>> GetSuperHero(int heroId)
	{
		using var connection = new SqlConnection(_config.GetConnectionString("Test"));
		var hero = await connection.QueryFirstAsync<SuperHero>("select * from SuperHero where id = @Id",
			new { Id = heroId });
		return Ok(hero);
	}

	[HttpPost]
	public async Task<ActionResult<List<SuperHero>>> CreateSuperHero(SuperHero hero)
	{
		using var connection = new SqlConnection(_config.GetConnectionString("Test"));
		await connection.ExecuteAsync("INSERT INTO SuperHero(Name, FirstName, LastName, Place) VALUES (@Name, @FirstName, @LastName, @Place)", hero);
		return Ok(await SelectAllHeroes(connection));
	}

	[HttpPut]
	public async Task<ActionResult<SuperHero>> UpdateSuperHero(SuperHero hero)
	{
		using var connection = new SqlConnection(_config.GetConnectionString("Test"));
		await connection.ExecuteAsync("update SuperHero set name = @Name, firstname = @FirstName, lastname = @LastName, place = @Place where id = @Id", hero);
		return Ok(await SelectAllHeroes(connection));
	}

	[HttpDelete("{heroId}")]
     public async Task<ActionResult<SuperHero>> DeleteSuperHero(int heroId)
    {
        using var connection = new SqlConnection(_config.GetConnectionString("Test"));
		await connection.ExecuteAsync("delete from SuperHero where id = @Id", new { Id = heroId});
        return Ok(await SelectAllHeroes(connection));
    }

    private static async Task<IEnumerable<SuperHero>> SelectAllHeroes(SqlConnection connection)
    {
        return await connection.QueryAsync<SuperHero>("select * from SuperHero");
    }
}