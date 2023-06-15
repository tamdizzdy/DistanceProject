namespace Distance_Calculate.Model;

public class UserUpdateDTO
{
    public int Id { get; set; } 
    public string UserName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DOB { get; set; }
    DateTime CreatedDate { get; set; }
}