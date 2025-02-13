using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


public class Subscriber
{
    public async Task Start()
    {
        // Create a connection factory
        var factory = new ConnectionFactory { HostName = "localhost" };

        // Create a connection
        using var connection = await factory.CreateConnectionAsync();

        // Create a channel
        using var channel = await connection.CreateChannelAsync();

        // Declare the queue
        var queueDeclareOk = await channel.QueueDeclareAsync(
            queue: "my_queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        // Create a consumer
        var consumer = new AsyncEventingBasicConsumer(channel);

        // Set up the message received event handler
        consumer.ReceivedAsync += async (sender, args) =>
        {
            try
            {
                // Get the message body
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Deserialize the message to a DTO object
                var dto = JsonSerializer.Deserialize<DTO>(message);

                // Print the DTO information to the console              
                Console.WriteLine($"Cell Number: {dto.CellNumber}");
     

                // Acknowledge the message processing (if needed)
                await channel.BasicAckAsync(args.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                // Handle exceptions (log or requeue the message)
                Console.WriteLine($"Error processing message: {ex.Message}");
            }

            // Ensure that a Task is returned
            await Task.CompletedTask;
        };

        // Start consuming messages
        await channel.BasicConsumeAsync(queue: "my_queue", autoAck: false, consumer: consumer);

        Console.WriteLine("RabbitMQ subscriber started. Press any key to exit...");
        await Task.Delay(-1); // Wait indefinitely
    }
}
