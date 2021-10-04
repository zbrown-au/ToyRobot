using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using ToyRobot.Models;
using ToyRobot.Repositories;
using ToyRobot.Services;

namespace ToyRobot
{
    public class Program
    {
        private const string COMMAND_NOT_VALID = "Command was not valid";

        public static void Main(string[] args)
        {
            // Dependency Injection Configuration
            var serviceProvider = new ServiceCollection()
                .AddMemoryCache()
                .AddTransient<IRobotService, RobotService>()
                .AddTransient<IRobotRepository, RobotRepository>()
                .BuildServiceProvider();

            var _robotService = serviceProvider.GetService<IRobotService>();
            
            // Program
            var commands = Enum.GetNames(typeof(CommandEnum));

            Console.WriteLine("Enter command for ROBOT: " + string.Join(",", commands));
            
            while (true)
            {
                var commandLine = Console.ReadLine().Split(' ');

                if (commandLine.Any())
                {
                    var commandText = commandLine[0];

                    if (commandText.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    // Validate Command
                    var isValidCommand = Enum.TryParse(commandText, true, out CommandEnum command);

                    if (isValidCommand)
                    {
                        CommandRequest commandRequest;
                            
                        if (command == CommandEnum.Place)
                        {
                            if (commandLine.Length == 2)
                            {
                                var commandArgs = commandLine[1].Split(',');
                                int x;
                                int y;

                                // Validate Place Command
                                if (commandArgs.Length >= 2)
                                {
                                    var isValidX = int.TryParse(commandArgs[0], out x);
                                    var isValidY = int.TryParse(commandArgs[1], out y);

                                    if (!isValidX || !isValidY)
                                    {
                                        Console.WriteLine(COMMAND_NOT_VALID);
                                        continue;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(COMMAND_NOT_VALID);
                                    continue;
                                }

                                var hasDirection = commandArgs.Length >= 3;
                                var isValidDirection = Enum.TryParse(hasDirection ? commandArgs[2].Trim() : string.Empty, true, out DirectionEnum direction);

                                // Create Request
                                commandRequest = new CommandRequest(command, new Position(x, y, isValidDirection ? direction : null));
                            }
                            else
                            {
                                Console.WriteLine(COMMAND_NOT_VALID);
                                continue;
                            }
                        }
                        else
                        {
                            // Create Request
                            commandRequest = new CommandRequest(command);
                        }

                        // Process Command Request
                        var result = _robotService.ProcessCommand(commandRequest);

                        // Display Result
                        if (!result.IsSuccess && !string.IsNullOrWhiteSpace(result.ErrorMessage))
                        {
                            Console.WriteLine(result.ErrorMessage);
                        }
                        else
                        {
                            if (command == CommandEnum.Report)
                            {
                                Console.WriteLine($"{result.Position.X},{result.Position.Y},{result.Position.Direction}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(COMMAND_NOT_VALID);
                    }                    
                }
            }            
        }
    }
}
