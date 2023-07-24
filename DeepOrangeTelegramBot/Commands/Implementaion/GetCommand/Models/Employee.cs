namespace DeepOrangeTelegramBot.Commands.Implementaion.GetEmployeeCommand.Models;

public class Employee
{
    public int EmployeeId { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public List<Technology>? Technologies { get; set; }
}