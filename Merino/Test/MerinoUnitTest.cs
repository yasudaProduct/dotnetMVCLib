namespace Merino.Test
{
    public class MerinoUnitTest : IDisposable
    {
        public void Dispose()
        {
            // 完了後にアンマネージドリソースの処理したり
            Console.WriteLine("disposed");
        }
    }
}
