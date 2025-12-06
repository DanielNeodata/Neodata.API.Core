using System;
using System.Collections.Generic;

namespace NeoAI
{
	public class NeoMind
	{
		public List<double> _before { get; set; }
		public List<double> _after { get; set; }
		public List<double> _response { get; set; }
		public int _columns { get; set; }
		public int _lines { get; set; }

		public double[,] _trainingInputs;
		public double[,] _expectedOutputs;

		public NeuralNetWork NN { get; set; }
		public NeoMind(int columns, int lines, int epochs)
		{
			/* Create NeuralNet */
			NN = new NeuralNetWork(columns, lines, epochs);

			/* Set values for the current instance */
			_columns = columns;
			_lines = lines;

			/* Set initialization values for SynapsesMatrix */
			_before = BuildListResult(NN.SynapsesMatrix);
			_after = new List<double>();
		}

		public void UpdateSynapsesMatrix(double[,] _newSynapseMatrix, bool _all)
		{
			if (_all) { NN.SynapsesMatrix = _newSynapseMatrix; }
			_after = BuildListResult(NN.SynapsesMatrix);
		}
		public bool AddItemForTrainning(double[,] arrItemTrainning, double[,] arrItemExpectedResult)
		{
			try
			{
				if (_trainingInputs == null)
				{
					_trainingInputs = arrItemTrainning;
					_expectedOutputs = arrItemExpectedResult;
				}
				else
				{
					_trainingInputs = Tools.ResizeArray(_trainingInputs, (_trainingInputs.GetLength(0) + 1), _lines);
					_expectedOutputs = Tools.ResizeArray(_expectedOutputs, 1, (_expectedOutputs.GetLength(1) + 1));

					int _sub1 = (_trainingInputs.GetLength(0) - 1);
					for (int i = 0; i < _trainingInputs.GetLength(1); i++)
					{
						_trainingInputs[_sub1, i] = arrItemTrainning[0, i];
					}
					_expectedOutputs[0, (_expectedOutputs.GetLength(1) - 1)] = arrItemExpectedResult[0, 0];
				}

				NN.Train(_trainingInputs, NeuralNetWork.MatrixTranspose(_expectedOutputs), NN.Epochs);
				UpdateSynapsesMatrix(NN.SynapsesMatrix, false);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		public bool ExecuteTrainning()
		{
			try
			{
				NN.Train(_trainingInputs, NeuralNetWork.MatrixTranspose(_expectedOutputs), NN.Epochs);
				UpdateSynapsesMatrix(NN.SynapsesMatrix, false);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		public bool ResolveProblem(double[,] problemData)
		{
			try
			{
				_response = BuildListResult(NN.Think(problemData));
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private List<double> BuildListResult(double[,] matrix)
		{
			List<double> _response = new List<double>();
			int rowLength = matrix.GetLength(0);
			int colLength = matrix.GetLength(1);

			for (int i = 0; i < rowLength; i++)
			{
				for (int j = 0; j < colLength; j++)
				{
					_response.Add(matrix[i, j]);
				}
			}
			return _response;
		}
	}
}
