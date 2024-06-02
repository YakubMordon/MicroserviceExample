using CarRentalService.Data.Entities;
using CarRentalService.Model;

namespace CarRentalService.Data;

/// <summary>
/// Інтерфейс, який визначає контракт для роботи з даними, необхідними для аренди автомобілів.
/// </summary>
public interface ICarRentalDbContext
{
    /// <summary>
    /// Асинхронний метод для отримання списку доступних автомобілів.
    /// </summary>
    /// <returns>Список доступних автомобілів.</returns>
    Task<IEnumerable<Car>> GetAvailableCarsFromDbAsync();

    /// <summary>
    /// Асинхронний метод для аренди автомобіля в базі даних.
    /// </summary>
    /// <param name="request">Модель запиту на арендування.</param>
    /// <returns>True, якщо аренда успішно здійснена; в іншому випадку False.</returns>
    Task<bool> OrderCarInDbAsync(OrderModel request);

    /// <summary>
    /// Асинхронний метод для повернення автомобіля в базу даних.
    /// </summary>
    /// <param name="request">Модель запиту на повернення автомобіля.</param>
    /// <returns>True, якщо автомобіль успішно повернуто; в іншому випадку False.</returns>
    Task<bool> ReturnCarFromDbAsync(ReturnModel request);

    /// <summary>
    /// Асинхронний метод для отримання списку орендованих автомобілів.
    /// </summary>
    /// <returns>Список орендованих автомобілів.</returns>
    Task<IEnumerable<Car>> GetRentedCarsFromDbAsync();

    /// <summary>
    /// Асинхронний метод для отримання історії оренди.
    /// </summary>
    /// <returns>Список записів історії оренди.</returns>
    Task<IEnumerable<Order>> GetRentalHistoryFromDbAsync();

    /// <summary>
    /// Асинхронний метод для отримання списку пошкоджених автомобілів.
    /// </summary>
    /// <returns>Список пошкоджених автомобілів.</returns>
    Task<IEnumerable<Car>> GetDamagedCarsFromDbAsync();

    /// <summary>
    /// Асинхронний метод для відмови у оренді.
    /// </summary>
    /// <param name="orderId">Ідентифікатор оренди.</param>
    Task RejectOrderFromDbAsync(int orderId);
}