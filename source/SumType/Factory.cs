using S = System;
using SL = System.Linq;
using STT = System.Threading.Tasks;

namespace LMMarsano.SumType {
	/**
	 * <summary>
	 * Static methods for handling sum types.
	 * </summary>
	 */
	public static class Factory {
		/**
		 * <summary>
		 * Convert to <see cref='SumType.Maybe{T}'/>.
		 * </summary>
		 * <param name="value">Nothing.</param>
		 * <typeparam name="T">Maybe type parameter.</typeparam>
		 * <returns>Nothing case.</returns>
		 */
		public static Maybe<T> Maybe<T>(Nothing value) where T : object => value;
		/**
		 * <summary>
		 * Convert to <see cref='SumType.Maybe{T}'/>.
		 * </summary>
		 * <param name="value">Value.</param>
		 * <typeparam name="T">Value type.</typeparam>
		 * <returns>Just case.</returns>
		 */
		public static Maybe<T> Maybe<T>(T value) where T : object => value;
		/**
		 * <summary>
		 * Convert to <see cref='SumType.Result{T}'/>.
		 * </summary>
		 * <param name="exception">An exception to convert.</param>
		 * <typeparam name="T">Type parameter.</typeparam>
		 * <returns>Error case.</returns>
		 */
		public static Result<T> Result<T>(S.Exception exception) where T : object => exception;
		/**
		 * <summary>
		 * Convert to <see cref='SumType.Result{T}'/>.
		 * </summary>
		 * <param name="value">Value to convert.</param>
		 * <typeparam name="T">Value type.</typeparam>
		 * <returns>Ok case.</returns>
		 */
		public static Result<T> Result<T>(T value) where T : object => value;
		/**
		 * <summary>
		 * Convert to <see cref='SumType.Either{TLeft, TRight}'/>.
		 * </summary>
		 * <param name="value"><typeparamref name="TLeft"/> value to convert.</param>
		 * <typeparam name="TLeft">Left type.</typeparam>
		 * <typeparam name="TRight">Right type.</typeparam>
		 * <returns>Left case.</returns>
		 */
		public static Either<TLeft, TRight> Either<TLeft, TRight>(TLeft value)
		where TLeft : object
		where TRight : object
		=> value;
		/**
		 * <summary>
		 * Convert to <see cref='SumType.Either{TLeft, TRight}'/>.
		 * </summary>
		 * <param name="value"><typeparamref name="TRight"/> value to convert.</param>
		 * <typeparam name="TLeft">Left type.</typeparam>
		 * <typeparam name="TRight">Right type.</typeparam>
		 * <returns>Right case.</returns>
		 */
		public static Either<TLeft, TRight> Either<TLeft, TRight>(TRight value)
		where TLeft : object
		where TRight : object
		=> value;
		/**
		 * <summary>
		 * Convert a task throwing exceptions to a task returning results.
		 * </summary>
		 * <param name="task">A task.</param>
		 * <typeparam name="T">A non-nullable type.</typeparam>
		 * <returns>A task returning results.</returns>
		 */
		public static STT.Task<Result<T>> ResultAsync<T>(STT.Task<T> task)
		where T : object
		=> task.ContinueWith
		   ((task)
		    => task.Status switch
		       { STT.TaskStatus.RanToCompletion => (Result<T>)task.Result
		       , STT.TaskStatus.Canceled => new S.AggregateException(SL.Enumerable.Prepend(SL.Enumerable.Empty<STT.TaskCanceledException>(), new STT.TaskCanceledException(task)))
		       , _ => task.Exception
		       }
		   );
		/**
		 * <summary>
		 * Convert a task throwing exceptions to a task returning results.
		 * </summary>
		 * <param name="task">A task.</param>
		 * <param name="onFailure">An map from unsuccessful tasks to results.</param>
		 * <typeparam name="T">A non-nullable type.</typeparam>
		 * <returns>A task returning results.</returns>
		 */
		public static STT.Task<Result<T>> ResultAsync<T>(STT.Task<T> task, S.Func<STT.Task<T>, Result<T>> onFailure)
		where T : object
		=> task.ContinueWith
		   ((task)
		    => task.Status switch
		       { STT.TaskStatus.RanToCompletion => (Result<T>)task.Result
		       , _ => onFailure(task)
		       }
		   );
		/**
		 * <summary>
		 * Evaluate <paramref name="function"/> to result.
		 * </summary>
		 * <param name="function">Function to evaluate.</param>
		 * <returns>Result of successful evaluation (<see cref="Nothing"/>) or error.</returns>
		 */
		public static Result<Nothing> Try(S.Action function) {
			try {
				function();
				return Nothing.Value;
			} catch (S.Exception error) {
				return error;
			}
		}
		/**
		 * <summary>
		 * Evaluate <paramref name="function"/> to result.
		 * </summary>
		 * <param name="function">Function to evaluate.</param>
		 * <param name="errorMap">Error mapper.</param>
		 * <returns>Result of successful evaluation (<see cref="Nothing"/>) or error.</returns>
		 */
		public static Result<Nothing> Try(S.Action function, S.Func<S.Exception, S.Exception> errorMap) {
			try {
				function();
				return Nothing.Value;
			} catch (S.Exception error) {
				return errorMap(error);
			}
		}
		/**
		 * <summary>
		 * Evaluate <paramref name="function"/> to result.
		 * </summary>
		 * <param name="function">Function to evaluate.</param>
		 * <typeparam name="T"><paramref name="function"/>’s image type.</typeparam>
		 * <returns>Result of successful evaluation or error.</returns>
		 */
		public static Result<T> Try<T>(S.Func<T> function) where T : object {
			try {
				return function();
			} catch (S.Exception error) {
				return error;
			}
		}
		/**
		 * <summary>
		 * Evaluate <paramref name="function"/> to result.
		 * </summary>
		 * <param name="function">Function to evaluate.</param>
		 * <param name="errorMap">Error mapper.</param>
		 * <typeparam name="T"><paramref name="function"/>’s image type.</typeparam>
		 * <returns>Result of successful evaluation or error.</returns>
		 */
		public static Result<T> Try<T>(S.Func<T> function, S.Func<S.Exception, S.Exception> errorMap)
		where T : object {
			try {
				return function();
			} catch (S.Exception error) {
				return errorMap(error);
			}
		}
		/**
		 * <summary>
		 * Map <paramref name="function"/> to a result returning function.
		 * </summary>
		 * <param name="function">Function to map.</param>
		 * <typeparam name="T"><paramref name="function"/>’s domain.</typeparam>
		 * <typeparam name="TResult"><paramref name="function"/>’s codomain.</typeparam>
		 * <returns>Function that returns results instead of throwing errors.</returns>
		 */
		public static S.Func<T, Result<TResult>> TryMap<T, TResult>(S.Func<T, TResult> function)
		where TResult : object
		=> (value)
		=> {
			try {
				return function(value);
			} catch (S.Exception error) {
				return error;
			}
		};
		/**
		 * <summary>
		 * Map <paramref name="function"/> to a result returning function.
		 * </summary>
		 * <param name="function">Function to map.</param>
		 * <param name="errorMap">Error mapper.</param>
		 * <typeparam name="T"><paramref name="function"/>’s domain.</typeparam>
		 * <typeparam name="TResult"><paramref name="function"/>’s codomain.</typeparam>
		 * <returns>Function that returns results instead of throwing errors.</returns>
		 */
		public static S.Func<T, Result<TResult>> TryMap<T, TResult>(S.Func<T, TResult> function, S.Func<S.Exception, S.Exception> errorMap)
		where TResult : object
		=> (value)
		=> {
			try {
				return function(value);
			} catch (S.Exception error) {
				return errorMap(error);
			}
		};
	}
}
