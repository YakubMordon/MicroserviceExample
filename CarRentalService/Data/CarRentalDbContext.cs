namespace CarRentalService.Data
{
    using Model;
    using Entities;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Клас-контекст для роботи з даними потрібними для арендування автомобіля
    /// </summary>
    public class CarRentalDbContext : DbContext, ICarRentalDbContext
    {
        /// <summary>
        /// Рядок підключення до бд
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Повертає instance <see cref="CarRentalDbContext"/>
        /// </summary>
        public CarRentalDbContext() {}

        /// <summary>
        /// Повертає instance <see cref="CarRentalDbContext"/>
        /// </summary>
        /// <param name="connectionString">Рядок підключення до бд</param>
        public CarRentalDbContext(string connectionString)
        {
            this._connectionString = connectionString;
        }

        /// <summary>
        /// Метод для створення підключення до бд
        /// </summary>
        /// <returns>Підключення до бд</returns>
        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        /// <summary>
        /// Метод заполучення списку всіх можливих автомобілів
        /// </summary>
        /// <returns>Список всіх можливих автомобілів</returns>
        public async Task<IEnumerable<Car>> GetAvailableCarsFromDbAsync()
        {
            using (var connection = this.CreateConnection())
            {
                await connection.OpenAsync();

                var query = "SELECT Id, Brand, Model, IsAvailable FROM Cars WHERE IsAvailable = 'True'";

                var cars = new List<Car>();

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            cars.Add(new Car
                            {
                                Id = reader.GetInt32(0),
                                Brand = reader.GetString(1),
                                Model = reader.GetString(2),
                                IsAvailable = reader.GetBoolean(3)
                            });
                        }
                    }
                }

                return cars;
            }
        }

        /// <summary>
        /// Метод для арендування автомобіля в бд
        /// </summary>
        /// <param name="request">Модель запиту на арендування</param>
        /// <returns>True якщо успішно добавлено, в іншому випадку False</returns>
        public async Task<bool> OrderCarInDbAsync(OrderModel request)
        {
            using (var connection = this.CreateConnection())
            {
                await connection.OpenAsync();

                // Check car availability
                var isCarAvailable = await IsCarAvailableAsync(request.CarId, connection);
                if (!isCarAvailable)
                {
                    return false;
                }

                // Insert order into database
                var orderId = await InsertOrderAsync(request, connection);

                // Update car availability
                await UpdateCarAvailabilityAsync(request.CarId, "False", connection);

                return true;
            }
        }

        /// <summary>
        /// Метод перевірки чи є автомобіль вільним для аренди
        /// </summary>
        /// <param name="carId">Ідентифікатор автомобіля</param>
        /// <param name="connection">SQL підключення</param>
        /// <returns>True якщо є вільним, False якщо ні</returns>
        private async Task<bool> IsCarAvailableAsync(int carId, SqlConnection connection)
        {
            var query = "SELECT IsAvailable FROM Cars WHERE Id = @CarId";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CarId", carId);
                var isAvailable = await command.ExecuteScalarAsync();
                return Convert.ToBoolean(isAvailable);
            }
        }

        /// <summary>
        /// Метод добавлення аренди до бази даних
        /// </summary>
        /// <param name="request">Модель запиту на аренду</param>
        /// <param name="connection">SQL підключення</param>
        /// <returns>Кількість добавлених рядків</returns>
        private async Task<int> InsertOrderAsync(OrderModel request, SqlConnection connection)
        {
            var query = "INSERT INTO [dbo].[Orders] (Id, CarId, UserId, OrderDate, IsCompleted) VALUES (@Id, @CarId, @UserId, @OrderDate, @IsCompleted); SELECT SCOPE_IDENTITY();";

            using (var command = new SqlCommand(query, connection))
            {
                int orderId = await GetNextIdAsync("[dbo].[Orders]", connection);
                command.Parameters.AddWithValue("@Id", orderId);
                command.Parameters.AddWithValue("@CarId", request.CarId);
                command.Parameters.AddWithValue("@UserId", request.UserId);
                command.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                command.Parameters.AddWithValue("@IsCompleted", false);

                var result = await command.ExecuteScalarAsync();

                return result is DBNull or null 
                    ? 0 
                    : Convert.ToInt32(result);
            }
        }

        /// <summary>
        /// Метод оновлення стану зайняття автомобіля
        /// </summary>
        /// <param name="carId">Ідентифікатор автомобіля</param>
        /// <param name="isAvailable">Стан на який потрібно оновити автомобіль</param>
        /// <param name="connection">SQL підключення</param>
        /// <returns>Виконане завдання</returns>
        private async Task UpdateCarAvailabilityAsync(int carId, string isAvailable, SqlConnection connection)
        {
            var query = "UPDATE Cars SET IsAvailable = @IsAvailable WHERE Id = @CarId";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@IsAvailable", isAvailable);
                command.Parameters.AddWithValue("@CarId", carId);
                await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Метод повернення автомобіля в бд
        /// </summary>
        /// <param name="request">Модель запиту на повернення автомобіля</param>
        /// <returns>True, якщо успішно повернено автомобіль, інакше False.</returns>
        public async Task<bool> ReturnCarFromDbAsync(ReturnModel request)
        {
            using (var connection = this.CreateConnection())
            {
                await connection.OpenAsync();

                var orderQuery = "SELECT CarId FROM Orders WHERE Id = @OrderId";
                using (var orderCommand = new SqlCommand(orderQuery, connection))
                {
                    orderCommand.Parameters.AddWithValue("@OrderId", request.OrderId);
                    var carId = await orderCommand.ExecuteScalarAsync();

                    if (carId is null)
                    {
                        return false;
                    }

                    var returnQuery = "INSERT INTO Returns (Id, CarId, ReturnDate, IsDamaged) VALUES (@Id, @CarId, @ReturnDate, @IsDamaged)";
                    using (var returnCommand = new SqlCommand(returnQuery, connection))
                    {
                        int orderId = await GetNextIdAsync("Returns", connection);
                        returnCommand.Parameters.AddWithValue("@Id", orderId);
                        returnCommand.Parameters.AddWithValue("@CarId", carId);
                        returnCommand.Parameters.AddWithValue("@ReturnDate", DateTime.Now);
                        returnCommand.Parameters.AddWithValue("@IsDamaged", request.IsDamaged);

                        await returnCommand.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Метод заполучення списку арендованих автомобілів
        /// </summary>
        /// <returns>Список арендованих автомобілів</returns>
        public async Task<IEnumerable<Car>> GetRentedCarsFromDbAsync()
        {
            using (var connection = this.CreateConnection())
            {
                await connection.OpenAsync();

                var query = "SELECT Id, Brand, Model FROM Cars WHERE IsAvailable = 'False'";

                var rentedCars = new List<Car>();

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            rentedCars.Add(new Car
                            {
                                Id = reader.GetInt32(0),
                                Brand = reader.GetString(1),
                                Model = reader.GetString(2)
                            });
                        }
                    }
                }

                return rentedCars;
            }
        }

        /// <summary>
        /// Метод заполучення історії арендувань
        /// </summary>
        /// <returns>Список історії арендувань</returns>
        public async Task<IEnumerable<Order>> GetRentalHistoryFromDbAsync()
        {
            using (var connection = this.CreateConnection())
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM [dbo].[Orders]";

                var rentalHistory = new List<Order>();

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var order = new Order
                            {
                                Id = reader.GetInt32(0),
                                CarId = reader.GetInt32(1),
                                UserId = reader.GetInt32(2),
                                OrderDate = reader.GetDateTime(3),
                                IsCompleted = reader.GetBoolean(5)
                            };

                            if (!reader.IsDBNull(4))
                            {
                                order.ReturnDate = reader.GetDateTime(4);
                            }

                            rentalHistory.Add(order);
                        }
                    }
                }

                return rentalHistory;
            }
        }

        /// <summary>
        /// Метод заполучення списку пошкоджених автомобілів
        /// </summary>
        /// <returns>Список пошкоджених автомобілів</returns>
        public async Task<IEnumerable<Car>> GetDamagedCarsFromDbAsync()
        {
            using (var connection = this.CreateConnection())
            {
                await connection.OpenAsync();

                var query = "SELECT CarId FROM Returns WHERE IsDamaged = 'True'";

                var damagedCars = new List<Car>();

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            damagedCars.Add(new Car
                            {
                                Id = reader.GetInt32(0)
                            });
                        }
                    }
                }

                return damagedCars;
            }
        }

        /// <summary>
        /// Метод відмовлення у аренді
        /// </summary>
        /// <param name="orderId">Ідентифікатор аренди</param>
        /// <returns>Рядок, який зазначає успішність операції</returns>
        public async Task RejectOrderFromDbAsync(int orderId)
        {
            using (var connection = this.CreateConnection())
            {
                await connection.OpenAsync();

                var deleteQuery = "DELETE FROM Orders WHERE Id = @OrderId";
                using (var command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Метод заполучення наступного ідентифікатора для рядка
        /// </summary>
        /// <param name="tableName">Назва таблиці</param>
        /// <param name="connection">SQL підключення</param>
        /// <returns>Наступний ідентифікатор</returns>
        private async Task<int> GetNextIdAsync(string tableName, SqlConnection connection)
        {
            var query = $"SELECT COUNT(*) FROM {tableName}";
            using (var command = new SqlCommand(query, connection))
            {
                int count = (int)await command.ExecuteScalarAsync();
                return count + 1;
            }
        }
    }
}
