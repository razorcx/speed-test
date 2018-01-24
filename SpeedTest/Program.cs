using System.Collections;
using System.Collections.Generic;
using Tekla.Structures;
using Tekla.Structures.Filtering;
using Tekla.Structures.Filtering.Categories;
using Tekla.Structures.Model;

namespace SpeedTest
{
	public class Program
	{
		static void Main(string[] args)
		{
			new SpeedTest().Run();
		}
	}

	public class SpeedTest
	{
		public void Run()
		{
			var model = new Model();

			//IMPORTANT!!! ModelObjectEnumerator.AutoFetch = true;
			var parts1 = model.GetAllParts(true);  //fastest!!!

			//USING THE FILTER EXPRESSIONS - ModelObjectEnumerator.AutoFetch = true
			var parts2 = model.GetParts(true);  //very fast with more flexibility

			//ModelObjectEnumerator.AutoFetch - ModelObjectEnumerator.AutoFetch = false
			var parts3 = model.GetAllParts(false);  //very slow

			//USING THE FILTER EXPRESSIONS - ModelObjectEnumerator.AutoFetch = false
			var parts4 = model.GetParts(false);  //very slower
		}
	}

	public static class ExtensionMethods
	{
		public static List<Part> GetAllParts(this Model model, bool autoFetch)
		{
			//IMPORTANT!!!
			ModelObjectEnumerator.AutoFetch = autoFetch;

			//parts
			var types = new[] { typeof(Beam), typeof(BentPlate),
				typeof(ContourPlate), typeof(PolyBeam) };

			var parts = model
				.GetModelObjectSelector()
				.GetAllObjectsWithType(types)
				.ToAList<Part>();

			return parts;
		}

		public static List<T> ToAList<T>(this IEnumerator enumerator)
		{
			var list = new List<T>();
			while (enumerator.MoveNext())
			{
				var current = (T)enumerator.Current;
				if (current != null)
					list.Add(current);
			}
			return list;
		}

		public static List<Part> GetParts(this Model model, bool autoFetch)
		{
			ObjectFilterExpressions.Type objectType = new ObjectFilterExpressions.Type();
			NumericConstantFilterExpression type =
				new NumericConstantFilterExpression(TeklaStructuresDatabaseTypeEnum.PART);

			var expression2 = new BinaryFilterExpression(objectType, NumericOperatorType.IS_EQUAL, type);

			BinaryFilterExpressionCollection filterCollection =
				new BinaryFilterExpressionCollection
				{
					new BinaryFilterExpressionItem(expression2, BinaryFilterOperatorType.BOOLEAN_AND),
				};

			//IMPORTANT!!!
			ModelObjectEnumerator.AutoFetch = autoFetch;

			return model
				.GetModelObjectSelector()
				.GetObjectsByFilter(filterCollection)
				.ToAList<Part>();
		}
	}
}
