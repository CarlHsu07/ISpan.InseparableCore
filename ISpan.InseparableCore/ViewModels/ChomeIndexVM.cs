using System;
using ISpan.InseparableCore.Models.DAL;
namespace ISpan.InseparableCore.ViewModels
{
	public class ChomeIndexVM
	{
        public IEnumerable<TMovies> showing { get; set; }
        public IEnumerable<TMovies> soon { get; set; }
    }
}

