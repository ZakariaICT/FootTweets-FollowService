using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System;
using FollowService.Repositories;
using Newtonsoft.Json;
using FollowService.Model;

namespace FollowService
{
    public class RabbitMQListener
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IFollowRepo _followRepo;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQListener(IConnection connection, IModel channel, IFollowRepo followRepo)
        {
            _connection = connection;
            _channel = channel;
            _followRepo = followRepo;
        }

        public RabbitMQListener(IServiceProvider serviceProvider, IConnection connection, IModel channel)
        {
            _serviceProvider = serviceProvider;
            _connection = connection;
            _channel = channel;
        }

        private IFollowRepo GetFollowRepo()
        {
            using var scope = _serviceProvider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IFollowRepo>();
        }

        public void StartListening(IConfiguration configuration)
        {
            Console.WriteLine("RabbitMQListener is now listening for user deletion messages...");
            if (_channel == null)
            {
                Console.WriteLine("_channel is null. Ensure it is properly initialized.");
                return;
            }

            _channel.QueueDeclare(queue: "user.deleted",
                      durable: true,  // Change this to match the existing queue
                      exclusive: false,
                      autoDelete: false,
                      arguments: null);


            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                HandleUserDeletion(ea, configuration);
            };

            _channel.BasicConsume(queue: "user.deleted",
                                   autoAck: true,
                                   consumer: consumer);
        }

        public void deleteUsersTweets(IConnection _connection, IModel _channel, IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(configuration["RabbitMQConnection"]),
            };

            // Initialize RabbitMQ connection and channel here (similar to previous code)
            // ...
            _channel.QueueDeclare(queue: "user.deleted",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                // Obtain the JSON message from the user.deletion queue
                byte[] messageBodyBytes = ea.Body.ToArray();
                string messageBody = Encoding.UTF8.GetString(messageBodyBytes);

                // Log that a user deletion request has been received
                Console.WriteLine("Received user deletion request:");

                // Deserialize the JSON message to extract the UserId
                var messageObject = JsonConvert.DeserializeObject<Follow>(messageBody);
                string uidAuth = messageObject.FollowerId;

                // Log the UID obtained from the message
                Console.WriteLine($"UID: {uidAuth}");

                // Delete media associated with the user ID from the database
                _followRepo.DeleteFollowContentById(uidAuth);

                // Log that media deletion has occurred
                Console.WriteLine($"Followings related to User with ID {uidAuth} deleted.");
            };

            _channel.BasicConsume(queue: "user.deleted",
                                   autoAck: true,
                                   consumer: consumer);
        }

        public void HandleUserDeletion(BasicDeliverEventArgs e, IConfiguration configuration)
        {
            try
            {
                byte[] messageBodyBytes = e.Body.ToArray();
                string messageBody = Encoding.UTF8.GetString(messageBodyBytes);
                Console.WriteLine($"Received message: {messageBody}");

                // Assuming the message format is just a simple JSON with UserId
                var messageObject = JsonConvert.DeserializeObject<UserDeletionMessage>(messageBody); // Replace with the correct class
                if (messageObject == null)
                {
                    Console.WriteLine("Deserialization of message failed.");
                    return;
                }

                string userId = messageObject.UserId; // Replace with the correct property
                Console.WriteLine($"Processing deletion for User ID: {userId}");

                // Check if _mediaRepo is null
                //if (_mediaRepo == null)
                //{
                //    Console.WriteLine("_mediaRepo is null. Repository is not initialized.");
                //    return;
                //}
                using (var scope = _serviceProvider.CreateScope())
                {
                    var mediaRepo = scope.ServiceProvider.GetRequiredService<IFollowRepo>();
                    mediaRepo.DeleteFollowContentById(userId); // This now uses a fresh context
                }
                Console.WriteLine($"Followings related to User with ID {userId} deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling user deletion message: {ex.Message}");
            }
        }
    }
}
