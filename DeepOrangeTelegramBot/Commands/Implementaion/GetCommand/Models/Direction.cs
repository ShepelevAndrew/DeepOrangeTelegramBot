namespace DeepOrangeTelegramBot.Commands.Implementaion.GetEmployeeCommand.Models;

public class Direction
{
    public int DirectionId { get; set; }
    public string? DirectionName { get; set; }
    public List<Technology>? Technologies { get; set; }
}