namespace BE.Data.Constand
{
	public static class ConfigEntity
	{
		public static class Rules
		{
			public const int MAX_LENGTH_TITLE = 300;
			public const string TABLE_NAME = "Rules";
		}
        public static class Notifications
        {
            public const int MAX_LENGTH_TITLE = 300;
            public const string TABLE_NAME = "Notification";
			public const int MAX_LENGTH_MESSAGE = 500;
        }

        public static class BlockingWeb
        {
            public const string TABLE_NAME = "BlockingWeb";
        }
        public static class Posts
        {
            public const string TABLE_NAME = "Posts";
        }
        public static class PostCategories
        {
            public const string TABLE_NAME = "PostCategories";
        }
        public static class PostComments
        {
            public const string TABLE_NAME = "PostComments";
        }
        public static class PostImages
        {
            public const string TABLE_NAME = "PostImages";
        }
        public static class StaffReviewTime
        {
            public const string TABLE_NAME = "StaffReviewTimes";
        }
    }
}
