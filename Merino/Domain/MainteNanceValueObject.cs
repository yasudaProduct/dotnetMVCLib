namespace Merino.Domain
{
    public class MainteNanceValueObject
    {
        public string CreatePgmId { get; protected set; }

        public int CreateUserId { get; protected set; }

        public DateTime CreateDate { get; protected set; }

        public string UpdatePgmId { get; protected set; }

        public int UpdateUserId { get; protected set; }

        public DateTime UpdateDate { get; protected set; }

        protected MainteNanceValueObject(int userId)
        {
            if (userId == 0) throw new ArgumentNullException(nameof(userId));

            this.CreateUserId = userId;
            this.UpdateUserId = userId;

            this.CreatePgmId = test();
            this.UpdatePgmId = test();

            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
        }

        private string test()
        {
            //TODO: 呼び出し元のメソッド名を取得する
            return "";
        }
    }
}
