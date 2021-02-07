using System.Collections;
using System.Threading.Tasks;

namespace Addler.Tests
{
    internal static class TaskExtensions
    {
        public static IEnumerator AsIEnumerator(this Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsFaulted && task.Exception != null)
            {
                throw task.Exception;
            }
        }
    }
}