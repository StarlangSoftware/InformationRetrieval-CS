namespace InformationRetrieval.Index
{
    public class Posting
    {
        protected int Id;

        public Posting(int id){
            Id = id;
        }

        public int GetId(){
            return Id;
        }
    }
}