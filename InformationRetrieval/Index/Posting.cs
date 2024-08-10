namespace InformationRetrieval.Index
{
    public class Posting
    {
        protected int Id;

        /// <summary>
        /// Constructor for the Posting class. Sets the document id attribute.
        /// </summary>
        /// <param name="id">Document id.</param>
        public Posting(int id){
            Id = id;
        }

        /// <summary>
        /// Accessor for the document id attribute.
        /// </summary>
        /// <returns>Document id.</returns>
        public int GetId(){
            return Id;
        }
    }
}