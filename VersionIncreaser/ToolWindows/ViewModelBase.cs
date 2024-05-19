using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VersionIncreaser.ToolWindows
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Событие, которое вызывается при изменении значения свойства.
        /// Реализует интерфейс INotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Вызвать событие: изменение свойства.
        /// Поскольку событие PropertyChanged может вызываться только из класса 
        /// в котором оно поределено, нам нужен этот метод, чтобы вызвать его из 
        /// другого места.
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            // Если событие PropertyChange определено вызвать его.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Установить значение поля свойства, при изменении значения и вызвать
        /// событие изменения значения свойства.
        /// </summary>
        /// <typeparam name="T">Тип параметров</typeparam>
        /// <param name="field">Поле, хранящее текущее значение</param>
        /// <param name="newValue">Новок значение</param>
        /// <param name="propertyName">Имя свойства, значение которого 
        /// мы переустанавливаем</param>
        /// <returns>Было ли изменено значение</returns>
        protected bool SetProperty<T>( ref T field,
            T newValue, [CallerMemberName] string? propertyName = null)
        {
            // Если значение совпадают, то
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                // Весруть ложь - значение не изменилось
                return false;
            }
            // Иначе установить новое значение.
            field = newValue;
            // Вызвать событие изменения значение свойства
            OnPropertyChanged(propertyName);
            // Весрнуть истину - значение обновилось.
            return true;
        }
    }
}
