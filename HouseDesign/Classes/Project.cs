using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HouseDesign.Classes
{
    [Serializable]
    public class Project
    {
        public Client Client { get; set; }
        public Scene Scene { get; set; }
        private Configuration configuration;
        public Currency Currency { get; set; }
        public float WallsHeight { get; set; }
        public Decimal Budget { get; set; }
        public String Notes { get; set; }
        public Decimal ActualPrice { get; set; }

        public UnitOfMeasurement MeasurementUnit{ get; set; }
        public bool IsEmpty { get; set; }

        public Project()
        {

        }
        public Project(Scene scene)
        {
            this.Scene = scene;
            this.IsEmpty = true;

        }
        public Project(Client client, Scene scene, Configuration configuration, Currency currency, float wallsHeight, Decimal budget, 
            String notes, UnitOfMeasurement unitOfMeasurement)
        {
            this.Client = client;
            this.Scene = scene;
            this.configuration = configuration;
            this.Currency = currency;
            this.WallsHeight = wallsHeight;
            this.Budget = budget;
            this.Notes = notes;
            this.MeasurementUnit = unitOfMeasurement;
            this.IsEmpty = false;
        }

        public Project GetClone()
        {
            Project clone = new Project();
            clone.Client = this.Client;
            clone.Currency = this.Currency;
            clone.ActualPrice = this.ActualPrice;
            clone.Budget = this.Budget;
            clone.configuration = this.configuration;
            clone.Notes = this.Notes;
            clone.Scene = this.Scene;
            clone.WallsHeight = this.WallsHeight;
            clone.IsEmpty = this.IsEmpty;

            return clone;
        }

        public Configuration GetConfiguration()
        {
            return this.configuration;
        }

        public enum UnitOfMeasurement
        {
            mm,
            cm,
            m
        }
    }
}
