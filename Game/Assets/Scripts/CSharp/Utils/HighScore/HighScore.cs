using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ServiceStack.Text;
using System;
namespace Utils {
	
	public class HighScore {
		
		public LinkedList<object[]> scores;
		//public LinkedList<Disctionary<string,object>> scores;
		
		public static LinkedList<object[]> CreateDefaultHighScore(){
			LinkedList<object[]> defaultHighScore = new LinkedList<object[]>();			
					
			for(int i = 0; i <10; i++) {
				
				defaultHighScore.AddLast(new object[] {char.ConvertFromUtf32(65+i), 0});
				//defaultHighScore.Add(char.ConvertFromUtf32(65+i), 42+i);
						
			}
			
			return defaultHighScore;
		}
		
		public HighScore(LinkedList<object[]> scores){			
			//scores = highScore.FromJson<Dictionary<string, int>>();
			this.scores = scores;		
			
			
		}
		
		public HighScore(string scoresString) {
			scores = scoresString.FromJson<LinkedList<object[]>>();
		}
		
		public bool isNewHighScore(int points) {
			foreach(object[] entry in scores)
				if(Convert.ToInt32(entry[1]) < points) return true;	
				
			return false;
		}
			
		public void AddNewEntry(string name, int points) {
			LinkedListNode<object[]> node = scores.First;
			
			while(!node.Next.Equals(scores.Last)) {
				if(Convert.ToInt32(node.Value[1]) <= points) {
					scores.AddBefore(node, new object[] {name, points});
					
					scores.RemoveLast();
					break;
				}
				node = node.Next;
			}
		}
		
		public string ToJSON(){
			return scores.ToJson();		
		}
	
	}
}
