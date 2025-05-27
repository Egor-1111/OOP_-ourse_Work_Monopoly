using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace BusinessLogic.Services.Interfaces
{
    public interface IGameUIHandler
    {
        Task ShowMessageAsync(string message);
        Task<string> GetInputAsync(string prompt);
        Task<int> ShowChoiceAsync(string title, params string[] options);
        Task UpdateLabelAsync(string text);
        void AddPlayerToBoard(string name, int index);
        Task UpdatePlayerPosition(string name, int position);

    }
}
