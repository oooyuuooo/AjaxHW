namespace WebApplication1.Models.DTO
{
	public class UserDto
	{
		public string? Name { get; set; }
		public int? Age { get; set; }
		public string? Email { get; set; }
		public IFormFile? Picture { get; set; }
    }
}
