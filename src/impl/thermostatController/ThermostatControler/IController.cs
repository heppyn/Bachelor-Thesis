using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermostatControler
{
    public interface IController
    {
        Users.Response Login(String name, String pass);
        bool CreateUser(String name, String pass);
        bool IsUserAdmin();

        void SaveSchedule(Schedule schedule);
        List<Schedule> GetSchedules();

        List<Profile> GetProfiles();

        Heaters FindHeaters();
        void SaveHeatersAndActiveProfile(List<Heater> heaters, string profileName);
        void SaveNewHeatersAndNonactiveProfile(List<Heater> heaters, string profileName);

        void SetReturnDateAndTemperature(DateTime date, int temp);
        void SetReturnDateAndProfile(DateTime date, Profile profile);
        void EarlyReturn();
        Dictionary<String, double> GetTemperature();
    }
}
