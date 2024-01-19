using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System;
using FollowService.Repositories;

namespace FollowService
{
    public class RabbitMQListener
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IFollowRepo _followRepo;

        public RabbitMQListener(IConnection connection, IModel channel, IFollowRepo followRepo)
        {
            _connection = connection;
            _channel = channel;
            _followRepo = followRepo;
        }

        public void Initialize()
        {
            // Initialize RabbitMQ connection and channel here (similar to previous code)
            // ...
            _channel.QueueDeclare(queue: "user.deletion",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                string userId = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(message).UserId;

                // Handle user deletion, including media, follow relationships, and content
                HandleUserDeletion(userId);

                Console.WriteLine($"Media, follow relationships, and content related to User with ID {userId} deleted.");
            };

            _channel.BasicConsume(queue: "user.deletion",
                                   autoAck: true,
                                   consumer: consumer);
        }

        private void HandleUserDeletion(string userId)
        {
            try
            {
                // Delete media associated with the user ID from the database
                // ...

                // Delete follow relationships related to the user ID from the database
                _followRepo.DeleteFollowContentById(userId);

                // Delete content related to the user ID from the content service

                Console.WriteLine($"Media, follow relationships, and content related to User with ID {userId} deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling user deletion message: {ex.Message}");
            }
        }
    }
}
