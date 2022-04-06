using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bankers Algorithm implementation.
/// <br></br>
/// Follows similarly to Section 7.5.3.1 of "Operating Systems Concepts with Java"
/// </summary>
/// 
/// <author>
/// Lucas Rendell
/// </author>
public class Bankers
{
    private int numberOfCustomers; // the number of customers
    private int numberOfResources; // the number of resources
    private int[] available; // the available amount of each resource
    private int[,] maximum; // the maximum demand of each customer
    private int[,] allocation; // the amount currently allocated to each customer
    private int[,] need; // the remaining needs of each customer

    public Bankers(int[] available, int[,] maximum, int[,] allocation, int numberOfCustomers, int numberOfResources)
    {
        this.available = available; // this was the only one constructed in the textbook? weird.
        this.maximum = maximum;
        this.allocation = allocation;
        this.numberOfCustomers = numberOfCustomers;
        this.numberOfResources = numberOfResources;
    }

    /// <summary>
    /// Calculates and returns the need matrix.
    /// </summary>
    public List<int> GetNeedMatrix()
    {
        List<int> val = new List<int>();
        need = new int[numberOfCustomers, numberOfResources];
        for (int i = 0; i < numberOfCustomers; i++)
        {
            for (int j = 0; j < numberOfResources; j++)
            {
                need[i, j] = maximum[i, j] - allocation[i, j];
                val.Add(need[i, j]);
            }
        }
        return val;
    }

    public List<string> DoEverything(int startIndex)
    {
        GetNeedMatrix(); //Asserts that this is calculated
        List<string> dialogue = new List<string>();
        List<int> safeSequence = new List<int>();
        int[] work = new int[available.Length];
        for (int i = 0; i < available.Length; i++)
        {
            work[i] = available[i];
        }
        int visited = 0;
        int broke = 0;
        int killswitch = 0;
        while (safeSequence.Count != numberOfCustomers || broke == 0)
        {
            if (killswitch >= 90000)
                break;
            killswitch++;

            for (int i = startIndex; i < numberOfCustomers; i++)
            {
                startIndex = 0;
                // Dialogue Formatting
                string s = "Process " + i + " Needs:";
                for (int j = 0; j < work.Length; j++)
                {
                    s = s + " " + need[i, j];
                }
                s += " Work:";
                for (int j = 0; j < work.Length; j++)
                {
                    s = s + " " + work[j];
                }

                // Process
                if (safeSequence.Contains(i))
                    continue;
                visited++;
                bool winner = true;
                for (int j = 0; j < work.Length; j++)
                {
                    if (need[i, j] > work[j])
                    {
                        winner = false;
                        break;
                    }
                }
                if (winner)
                {
                    visited = 0;
                    for (int j = 0; j < work.Length; j++)
                    {
                        work[j] += allocation[i, j];
                    }
                    s += " Safe! ";
                    safeSequence.Add(i);
                }
                else if (visited >= numberOfCustomers - safeSequence.Count)
                {
                    broke = i+1;
                    break;
                }
                else
                {
                    s += " Unsafe! ";
                }
                dialogue.Add(s);
            }
        }
        if (broke != 0)
        {
            dialogue.Add("Bankers confirmed unsatisfied on process " + (broke - 1) + " after " + safeSequence.Count + " safe processes were ran.");
            // TODO:
        }
        else
        {
            string s = "";
            foreach (int item in safeSequence)
            {
                s += item + " ";
            }
            dialogue.Add("Success! Safe sequence: " + s);
        }

        return dialogue;
    }
}
