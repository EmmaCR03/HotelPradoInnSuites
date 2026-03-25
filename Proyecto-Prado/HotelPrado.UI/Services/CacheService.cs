using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;

namespace HotelPrado.UI.Services
{
    /// <summary>
    /// Servicio de caché en memoria usando MemoryCache
    /// Alternativa ligera a Redis para aplicaciones pequeñas/medianas
    /// </summary>
    public class CacheService
    {
        private static readonly MemoryCache _cache = MemoryCache.Default;
        private static readonly object _lockObject = new object();
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Obtiene un valor del caché
        /// </summary>
        public T Obtener<T>(string clave) where T : class
        {
            try
            {
                var valor = _cache.Get(clave) as T;
                return valor;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Guarda un valor en el caché con expiración
        /// </summary>
        public void Guardar<T>(string clave, T valor, TimeSpan? tiempoExpiracion = null) where T : class
        {
            try
            {
                var politica = new CacheItemPolicy();
                
                if (tiempoExpiracion.HasValue)
                {
                    politica.AbsoluteExpiration = DateTimeOffset.Now.Add(tiempoExpiracion.Value);
                }
                else
                {
                    // Expiración por defecto: 1 hora
                    politica.AbsoluteExpiration = DateTimeOffset.Now.AddHours(1);
                }

                // Remover del caché si existe
                if (_cache.Contains(clave))
                {
                    _cache.Remove(clave);
                }

                _cache.Add(clave, valor, politica);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar en caché: {ex.Message}");
            }
        }

        /// <summary>
        /// Remueve un valor del caché
        /// </summary>
        public void Remover(string clave)
        {
            try
            {
                if (_cache.Contains(clave))
                {
                    _cache.Remove(clave);
                }
            }
            catch
            {
                // Ignorar errores
            }
        }

        /// <summary>
        /// Limpia todo el caché
        /// </summary>
        public void LimpiarTodo()
        {
            try
            {
                // Obtener todas las claves y removerlas
                var enumerator = _cache.GetEnumerator();
                var claves = new List<string>();
                
                while (enumerator.MoveNext())
                {
                    claves.Add(enumerator.Current.Key);
                }

                foreach (var clave in claves)
                {
                    _cache.Remove(clave);
                }
            }
            catch
            {
                // Ignorar errores
            }
        }

        /// <summary>
        /// Verifica si existe una clave en el caché
        /// </summary>
        public bool Existe(string clave)
        {
            return _cache.Contains(clave);
        }

        /// <summary>
        /// Obtiene o crea un valor del caché (patrón GetOrSet)
        /// </summary>
        public T ObtenerOCrear<T>(string clave, Func<T> funcionCreacion, TimeSpan? tiempoExpiracion = null) where T : class
        {
            var valor = Obtener<T>(clave);
            
            if (valor == null)
            {
                lock (_lockObject)
                {
                    // Verificar nuevamente después del lock (double-check pattern)
                    valor = Obtener<T>(clave);
                    if (valor == null)
                    {
                        valor = funcionCreacion();
                        if (valor != null)
                        {
                            Guardar(clave, valor, tiempoExpiracion);
                        }
                    }
                }
            }

            return valor;
        }

        /// <summary>
        /// Obtiene o crea un valor del caché de forma asíncrona
        /// </summary>
        public async Task<T> ObtenerOCrearAsync<T>(string clave, Func<Task<T>> funcionCreacion, TimeSpan? tiempoExpiracion = null) where T : class
        {
            var valor = Obtener<T>(clave);
            
            if (valor == null)
            {
                // Usar SemaphoreSlim para evitar deadlocks en lugar de lock + .Result
                await _semaphore.WaitAsync().ConfigureAwait(false);
                try
                {
                    // Double-check después de adquirir el semáforo
                    valor = Obtener<T>(clave);
                    if (valor == null)
                    {
                        valor = await funcionCreacion().ConfigureAwait(false);
                        if (valor != null)
                        {
                            Guardar(clave, valor, tiempoExpiracion);
                        }
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            return valor;
        }

        /// <summary>
        /// Invalida caché por patrón de clave
        /// </summary>
        public void InvalidarPorPatron(string patron)
        {
            try
            {
                var enumerator = _cache.GetEnumerator();
                var claves = new List<string>();
                
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Key.Contains(patron))
                    {
                        claves.Add(enumerator.Current.Key);
                    }
                }

                foreach (var clave in claves)
                {
                    _cache.Remove(clave);
                }
            }
            catch
            {
                // Ignorar errores
            }
        }
    }
}

