using System.Collections.Generic;
using System.Linq;
using DomainLayer.Models;

namespace ApplicationLayer.Optimizations
{
    public static class TaskCalculations
    {
        // Diccionario para almacenar en caché el porcentaje calculado.
        // La clave se genera en función de la cantidad total y completada.
        private static readonly Dictionary<string, double> _completionRateCache = new();

        /// <summary>
        /// Calcula y retorna el porcentaje de tareas completadas.
        /// Se usa memoización para evitar recalcular si la entrada es la misma.
        /// </summary>
        /// <param name="tasks">Listado de tareas</param>
        /// <returns>Porcentaje de tareas completadas</returns>
        public static double CalculateCompletionRate(IEnumerable<Tareas> tasks)
        {
            int total = tasks.Count();
            if (total == 0)
                return 0;

            // Suponemos que las tareas completadas tienen Status igual a "Completed" (ajusta según corresponda)
            int completed = tasks.Count(t => t.Status.ToLower() == "completed");

            // Genera una clave simple en función de los totales (si el listado cambia, la clave cambiará)
            string key = $"{total}_{completed}";

            if (_completionRateCache.TryGetValue(key, out double cachedRate))
            {
                return cachedRate;
            }

            double rate = (double)completed / total * 100;
            _completionRateCache[key] = rate;
            return rate;
        }

        /// <summary>
        /// Permite limpiar la caché si se realizan cambios en las tareas.
        /// </summary>
        public static void ClearCache()
        {
            _completionRateCache.Clear();
        }
    }
}
