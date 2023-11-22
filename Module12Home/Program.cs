using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class Car
{
    public string Model { get; set; }
    public int Speed { get; set; }
    public int Position { get; set; }

    public event EventHandler<string> Finish;

    public Car(string model, int speed)
    {
        Model = model;
        Speed = speed;
        Position = 0;
    }

    public virtual void Move()
    {
        Position += Speed;

        if (Position >= 100)
        {
            OnFinish($"{Model} финишировал!");
        }
    }

    protected virtual void OnFinish(string message)
    {
        Finish?.Invoke(this, message);
    }
}

public class SportsCar : Car
{
    public SportsCar(string model) : base(model, new Random().Next(10, 20)) { }
}

public class PassengerCar : Car
{
    public PassengerCar(string model) : base(model, new Random().Next(8, 15)) { }
}

public class Truck : Car
{
    public Truck(string model) : base(model, new Random().Next(5, 12)) { }
}

public class Bus : Car
{
    public Bus(string model) : base(model, new Random().Next(4, 10)) { }
}

public class RaceGame
{
    public delegate void RaceStartDelegate();
    public delegate void RaceFinishDelegate(string message);

    public event RaceStartDelegate RaceStart;
    public event RaceFinishDelegate RaceFinish;

    public void StartRace(params Car[] cars)
    {
        RaceStart?.Invoke();

        while (true)
        {
            foreach (var car in cars)
            {
                car.Move();
            }
        }
    }

    public void SubscribeToEvents(Car car)
    {
        car.Finish += (sender, message) => RaceFinish?.Invoke(message);
    }
}

class Program
{
    static void Main()
    {
        RaceGame raceGame = new RaceGame();

        SportsCar sportsCar = new SportsCar("Спортивный автомобиль");
        PassengerCar passengerCar = new PassengerCar("Легковой автомобиль");
        Truck truck = new Truck("Грузовик");
        Bus bus = new Bus("Автобус");

        raceGame.SubscribeToEvents(sportsCar);
        raceGame.SubscribeToEvents(passengerCar);
        raceGame.SubscribeToEvents(truck);
        raceGame.SubscribeToEvents(bus);

        raceGame.RaceStart += () => Console.WriteLine("Гонка началась!");
        raceGame.RaceFinish += message =>
        {
            Console.WriteLine(message);
            Console.WriteLine("Гонка завершена!");
            Environment.Exit(0);
        };

        raceGame.StartRace(sportsCar, passengerCar, truck, bus);
    }
}
