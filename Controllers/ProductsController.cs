using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Api.Models;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly IConfiguration _config;

    public ProductsController(IConfiguration config)
    {
        _config = config;
    }

    // GET /products
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        await using var conn = new SqlConnection(_config.GetConnectionString("Default"));
        await conn.OpenAsync();

        var cmd = new SqlCommand("SELECT Id, Name, Price FROM Products", conn);
        var reader = await cmd.ExecuteReaderAsync();

        var list = new List<object>();
        while (await reader.ReadAsync())
        {
            list.Add(new
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2)
            });
        }

        return Ok(list);
    }

    // POST /products
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductRequest request)
    {
        await using var conn = new SqlConnection(_config.GetConnectionString("Default"));
        await conn.OpenAsync();

        var cmd = new SqlCommand(
            "INSERT INTO Products (Name, Price) VALUES (@n, @p)", conn);

        cmd.Parameters.Add("@n", SqlDbType.NVarChar, 200).Value = request.Name;
        cmd.Parameters.Add("@p", SqlDbType.Decimal).Value = request.Price;

        await cmd.ExecuteNonQueryAsync();
        return Ok(new { message = "Product created successfully" });
    }

    // PUT /products/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductRequest request)
    {
        await using var conn = new SqlConnection(_config.GetConnectionString("Default"));
        await conn.OpenAsync();

        var cmd = new SqlCommand(
            "UPDATE Products SET Name = @n, Price = @p WHERE Id = @id", conn);

        cmd.Parameters.Add("@n", SqlDbType.NVarChar, 200).Value = request.Name;
        cmd.Parameters.Add("@p", SqlDbType.Decimal).Value = request.Price;
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

        var rows = await cmd.ExecuteNonQueryAsync();
        if (rows == 0) return NotFound(new { message = "Product not found" });

        return Ok(new { message = "Product updated successfully" });
    }

    // DELETE /products/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await using var conn = new SqlConnection(_config.GetConnectionString("Default"));
        await conn.OpenAsync();

        var cmd = new SqlCommand("DELETE FROM Products WHERE Id = @id", conn);
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

        var rows = await cmd.ExecuteNonQueryAsync();
        if (rows == 0) return NotFound(new { message = "Product not found" });

        return Ok(new { message = "Product deleted successfully" });
    }
}
