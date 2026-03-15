Information Retrieval
============

Video Lectures
============

[<img src=https://github.com/StarlangSoftware/InformationRetrieval/blob/master/video1.jpg width="50%">](https://youtu.be/DhjZPVrvdnE)[<img src=https://github.com/StarlangSoftware/InformationRetrieval/blob/master/video2.jpg width="50%">](https://youtu.be/rfNoyFw-_g8)[<img src=https://github.com/StarlangSoftware/InformationRetrieval/blob/master/video3.jpg width="50%">](https://youtu.be/sYHVpTZL6o4)[<img src=https://github.com/StarlangSoftware/InformationRetrieval/blob/master/video4.jpg width="50%">](https://youtu.be/bRckCK9VcKQ)[<img src=https://github.com/StarlangSoftware/InformationRetrieval/blob/master/video5.jpg width="50%">](https://youtu.be/ZX4zTT69ll0)[<img src=https://github.com/StarlangSoftware/InformationRetrieval/blob/master/video6.jpg width="50%">](https://youtu.be/AVoLka-LDXY)[<img src=https://github.com/StarlangSoftware/InformationRetrieval/blob/master/video7.jpg width="50%">](https://youtu.be/5GOyBTeSJwo)[<img src=https://github.com/StarlangSoftware/InformationRetrieval/blob/master/video8.jpg width="50%">](https://youtu.be/-iu6N8KZslw)[<img src=https://github.com/StarlangSoftware/InformationRetrieval/blob/master/video9.jpg width="50%">](https://youtu.be/LwQYHFyDd8U)[<img src=https://github.com/StarlangSoftware/InformationRetrieval/blob/master/video10.jpg width="50%">](https://youtu.be/Y_jS03r6GMI)[<img src=https://github.com/StarlangSoftware/InformationRetrieval/blob/master/video11.jpg width="50%">](https://youtu.be/msRT2yx0yms)[<img src=https://github.com/StarlangSoftware/InformationRetrieval/blob/master/video12.jpg width="50%">](https://youtu.be/B5RProYhMvk)[<img src=https://github.com/StarlangSoftware/InformationRetrieval/blob/master/video13.jpg width="50%">](https://youtu.be/dxc3ONoW63E)

For Developers
============

You can also see [Python](https://github.com/starlangsoftware/InformationRetrieval-Py), [Cython](https://github.com/starlangsoftware/InformationRetrieval-Cy), [C++](https://github.com/starlangsoftware/InformationRetrieval-CPP), [Js](https://github.com/starlangsoftware/InformationRetrieval-Js), [C](https://github.com/starlangsoftware/InformationRetrieval-C), or [Java](https://github.com/starlangsoftware/InformationRetrieval) repository.

For Contibutors
============

### Resources
1. Add resources to the project directory. Do not forget to choose 'EmbeddedRecource' in 'Build Action' and 'Copy always' in 'Copy to output directory' in File Properties dialog. 
   
### C# files
1. Do not forget to comment each function.
```
	/**
	* <summary>Returns the first literal's name.</summary>
	*
	* <returns>the first literal's name.</returns>
	*/
	public string Representative()
	{
		return GetSynonym().GetLiteral(0).GetName();
	}
```
2. Function names should follow pascal caml case.
```
	public string GetLongDefinition()
```
3. Write ToString methods, if necessary.
4. Use var type as a standard type.
```
	public override bool Equals(object second)
	{
		var relation = (Relation) second;
```
5. Use standard naming for private and protected class variables. Use _ for private and capital for protected class members.
```
    public class SynSet
    {
        private string _id;
		protected string Name;
```
6. Use NUnit for writing test classes. Use test setup if necessary.
```
   public class WordNetTest
    {
        WordNet.WordNet turkish;

        [SetUp]
        public void Setup()
        {
            turkish = new WordNet.WordNet();
        }

        [Test]
        public void TestSynSetList()
        {
            var literalCount = 0;
            foreach (var synSet in turkish.SynSetList()){
                literalCount += synSet.GetSynonym().LiteralSize();
            }
            Assert.AreEqual(110259, literalCount);
        }
```
