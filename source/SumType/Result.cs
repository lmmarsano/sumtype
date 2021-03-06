using S = System;
using SC = System.Collections;
using SCG = System.Collections.Generic;
using SDC = System.Diagnostics.CodeAnalysis;
using STT = System.Threading.Tasks;

namespace LMMarsano.SumType {
	/**
	 * <summary>
	 * Represents a possible value or error.
	 * </summary>
	 */
#pragma warning disable CS0660, CS0661 //defines operator == or operator != but does not override Object.Equals(object o) Object.GetHashCode()
	public abstract class Result<T>: SCG.IEnumerable<T>, S.IEquatable<Result<T>> where T : object {
#pragma warning restore CS0660, CS0661
		///
		public static implicit operator Result<T>(T value) => new Ok<T>(value);
		///
		public static implicit operator Result<T>(S.Exception exception) => new Error<T>(exception);
		/**
		 * <summary>
		 * Equality relation.
		 * </summary>
		 * <param name="a">Operand.</param>
		 * <param name="b">Operand.</param>
		 * <returns>Truth value.</returns>
		 */
		public static bool operator ==(Result<T> a, Result<T> b) => Equals(a, b);
		/**
		 * <summary>
		 * Inequality relation.
		 * </summary>
		 * <param name="a">Operand.</param>
		 * <param name="b">Operand.</param>
		 * <returns>Truth value.</returns>
		 */
		public static bool operator !=(Result<T> a, Result<T> b) => !(a == b);
		/// <summary>Result as task.</summary>
		public abstract STT.Task<T> ToTask { get; }
		/**
		 * <summary>
		 * Project the possible value.
		 * </summary>
		 * <param name="map">The map to apply to the possible value.</param>
		 * <typeparam name="TResult">The map’s image type.</typeparam>
		 * <returns>A <see cref="Result{TResult}"/> whose possible element is any input element’s map projection.</returns>
		 */
		public abstract Result<TResult> Select<TResult>(S.Func<T, TResult> map) where TResult : object;
		/**
		 * <summary>
		 * Project the possible error.
		 * </summary>
		 * <param name="map">The map to apply to the possible error.</param>
		 * <returns>A <see cref="Result{TResult}"/> whose possible error is a map projection.</returns>
		 */
		public abstract Result<T> SelectError(S.Func<S.Exception, S.Exception> map);
		/**
		 * <summary>
		 * Project and flatten the possible value.
		 * </summary>
		 * <param name="map">The map to apply to the possible value.</param>
		 * <typeparam name="TResult">The map’s possible image type.</typeparam>
		 * <returns>A <see cref="Result{TResult}"/> whose value is any input element’s map projection.</returns>
		 */
		public abstract Result<TResult> SelectMany<TResult>(S.Func<T, Result<TResult>> map) where TResult : object;
		/**
		 * <summary>
		 * Sequential composition: replace current value, if any, with <paramref name="next"/>.
		 * </summary>
		 * <param name="next">The next value.</param>
		 * <typeparam name="TResult">Next internal type.</typeparam>
		 * <returns>Next value or <see cref='Error{TResult}'/>.</returns>
		 */
		public abstract Result<TResult> Combine<TResult>(Result<TResult> next) where TResult : object;
		/**
		 * <summary>
		 * Filters a <see cref='Result{T}'/> of values based on a predicate.
		 * </summary>
		 * <param name="predicate">A function to test each element for a condition.</param>
		 * <param name="onError">An error map for values that fail <paramref name="predicate"/>.</param>
		 * <returns>A <see cref='Result{T}'/> that contains elements from the input sequence that satisfy the condition.</returns>
		 */
		public abstract Result<T> Where(S.Func<T, bool> predicate, S.Func<T, S.Exception> onError);
		/**
		 * <summary>
		 * Handle <see cref='Error{T}'/>s.
		 * </summary>
		 * <param name="handler">Exception handler.</param>
		 * <returns>Current value if <see cref='Ok{T}'/>, otherwise, the image of <paramref name="handler"/>.</returns>
		 */
		public abstract Result<T> Catch(S.Func<Error<T>, Result<T>> handler);
		/**
		 * <summary>
		 * Extract the possible value or a default alternative.
		 * </summary>
		 * <param name="alternative">Fallback for nothing.</param>
		 * <returns>The possible element or <paramref name="alternative"/> fallback.</returns>
		 */
		public abstract T Reduce(T alternative);
		/**
		 * <summary>
		 * Lazily extract the possible value or error value alternatives.
		 * </summary>
		 * <param name="alternative">Function that maps error values to alternatives.</param>
		 * <returns>The possible element or fallback mapped from error.</returns>
		 */
		public abstract T Reduce(S.Func<S.Exception, T> alternative);
		/**
		 * <summary>
		 * Filter values of <see cref="Result{TResult}"/> based on a specified type.
		 * </summary>
		 * <param name="onError">Maps Ok type to Error type when Ok is filtered out.</param>
		 * <typeparam name="TResult">Type to filter on.</typeparam>
		 * <returns>A <see cref="Result{TResult}"/> that contains the values from input of type <typeparamref name="TResult"/>.</returns>
		 */
		public abstract Result<TResult> OfType<TResult>(S.Func<T, S.Exception> onError) where TResult : object;
		/**
		 * <summary>
		 * Asynchronously map a possible value to a possible result.
		 * </summary>
		 * <param name="map">Asynchronous map.</param>
		 * <typeparam name="TResult">Type of possible result value.</typeparam>
		 * <returns>A task yielding possible value.</returns>
		 */
		public abstract STT.Task<Result<TResult>> TraverseAsync<TResult>(S.Func<T, STT.Task<TResult>> map) where TResult : object;
		/**
		 * <summary>
		 * Enumerate possible value.
		 * </summary>
		 * <returns>Singleton or empty sequence.</returns>
		 */
		public abstract SCG.IEnumerator<T> GetEnumerator();
		SC.IEnumerator SC.IEnumerable.GetEnumerator() => GetEnumerator();
		/**
		 * <summary>
		 * Equals predicate.
		 * </summary>
		 * <param name="other">Operand.</param>
		 * <returns>Truth value.</returns>
		 */
		public abstract bool Equals(Result<T> other);
	}
	/**
	 * <summary>
	 * The successful value.
	 * </summary>
	 */
	public sealed class Ok<T>: Result<T>, S.IEquatable<Ok<T>> where T : object {
		///
		public static implicit operator Ok<T>(T value) => new Ok<T>(value);
		///
		public static implicit operator T(Ok<T> value) => value.Value;
		/// <inheritdoc cref='Result{T}.operator ==' />
		public static bool operator ==(Ok<T> a, Ok<T> b) => a.Equals(b);
		/// <inheritdoc cref='Result{T}.operator !=' />
		public static bool operator !=(Ok<T> a, Ok<T> b) => !(a == b);
		/**
		 * <summary>
		 * Embedded value.
		 * </summary>
		 * <value>The successful value.</value>
		 */
		public T Value { get; }
		/// <inheritdoc/>
		public override STT.Task<T> ToTask => STT.Task.FromResult(Value);
		/**
		 * <summary>
		 * Construct Ok case.
		 * </summary>
		 * <param name="value">Value of Ok case.</param>
		 */
		public Ok(T value) {
			Value = value;
		}
		/**
		 * <summary>
		 * Extract value from Ok case.
		 * </summary>
		 * <param name="value">Value to extract.</param>
		 */
		public void Deconstruct(out T value) {
			value = Value;
		}
		/// <inheritdoc/>
		public override Result<TResult> Select<TResult>(S.Func<T, TResult> map) => map(Value);
		/// <inheritdoc/>
		public override Result<T> SelectError(S.Func<S.Exception, S.Exception> map) => this;
		/// <inheritdoc/>
		public override Result<TResult> SelectMany<TResult>(S.Func<T, Result<TResult>> map) => map(Value);
		/// <inheritdoc/>
		public override Result<T> Where(S.Func<T, bool> predicate, S.Func<T, S.Exception> onError)
		=> predicate(Value)
		 ? (Result<T>)this
		 : onError(Value);
		/// <inheritdoc/>
		public override Result<TResult> Combine<TResult>(Result<TResult> next) => next;
		/// <inheritdoc/>
		public override Result<T> Catch(S.Func<Error<T>, Result<T>> handler) => this;
		/// <inheritdoc/>
		public override T Reduce(T alternative) => Value;
		/// <inheritdoc/>
		public override T Reduce(S.Func<S.Exception, T> alternative) => Value;
		/// <inheritdoc cref='Result{T}.Equals(Result{T})'/>
		public bool Equals(Ok<T> other)
		=> ReferenceEquals(this, other)
		|| Value.Equals(other.Value);
		/// <inheritdoc/>
		public override bool Equals(Result<T> other) => Equals((object)other);
		/// <inheritdoc/>
		public override bool Equals(object obj) => (obj as Ok<T>)?.Equals(this) == true;
		/// <inheritdoc/>
		public override int GetHashCode() => Value.GetHashCode();
		/// <inheritdoc/>
		public override SCG.IEnumerator<T> GetEnumerator() {
			yield return Value;
		}
		/// <inheritdoc/>
		public override Result<TResult> OfType<TResult>(S.Func<T, S.Exception> onError)
		=> Value is TResult result
		 ? (Result<TResult>)result
		 : onError(Value);
		/// <inheritdoc/>
		public override string ToString() => $"Ok<{typeof(T)}>({Value})";
		/// <inheritdoc/>
		public override STT.Task<Result<TResult>> TraverseAsync<TResult>(S.Func<T, STT.Task<TResult>> map)
		=> map(Value).ToResultAsync();
	}
	/**
	 * <summary>
	 * The error.
	 * </summary>
	 */
	public sealed class Error<T>: Result<T>, S.IEquatable<Error<T>> where T : object {
		///
		public static implicit operator Error<T>(S.Exception value) => new Error<T>(value);
		///
		public static implicit operator S.Exception(Error<T> value) => value.Value;
		/// <inheritdoc cref='Result{T}.operator =='/>
		public static bool operator ==(Error<T> a, Error<T> b) => a.Equals(b);
		/// <inheritdoc cref='Result{T}.operator !='/>
		public static bool operator !=(Error<T> a, Error<T> b) => !(a == b);
		/**
		 * <summary>
		 * Embedded value.
		 * </summary>
		 * <value>The error.</value>
		 */
		public S.Exception Value { get; }
		/// <inheritdoc/>
		public override STT.Task<T> ToTask => STT.Task.FromException<T>(Value);
		/**
		 * <summary>
		 * Construct Error case.
		 * </summary>
		 * <param name="value">Exception for Error case.</param>
		 */
		public Error(S.Exception value) {
			Value = value;
		}
		/**
		 * <summary>
		 * Extract exception from Error case.
		 * </summary>
		 * <param name="value">Exception to extract.</param>
		 */
		public void Deconstruct(out S.Exception value) {
			value = Value;
		}
		/// <inheritdoc/>
		public override Result<TResult> Select<TResult>(S.Func<T, TResult> map) => Value;
		/// <inheritdoc/>
		public override Result<T> SelectError(S.Func<S.Exception, S.Exception> map) => map(Value);
		/// <inheritdoc/>
		public override Result<TResult> SelectMany<TResult>(S.Func<T, Result<TResult>> map) => Value;
		/// <inheritdoc/>
		public override Result<T> Where(S.Func<T, bool> predicate, S.Func<T, S.Exception> onError) => this;
		/// <inheritdoc/>
		public override Result<TResult> Combine<TResult>(Result<TResult> next) => Value;
		/// <inheritdoc/>
		public override Result<T> Catch(S.Func<Error<T>, Result<T>> handler) => handler(Value);
		/// <inheritdoc/>
		public override T Reduce(T alternative) => alternative;
		/// <inheritdoc/>
		public override T Reduce(S.Func<S.Exception, T> alternative) => alternative(Value);
		/// <inheritdoc cref='Result{T}.Equals(Result{T})'/>
		public bool Equals(Error<T> other)
		=> ReferenceEquals(this, other)
		|| Value.Equals(other.Value);
		/// <inheritdoc/>
		public override bool Equals(Result<T> other) => Equals((object)other);
		/// <inheritdoc/>
		public override bool Equals(object obj) => (obj as Error<T>)?.Equals(this) == true;
		/// <inheritdoc/>
		public override int GetHashCode() => Value.GetHashCode();
		/// <inheritdoc/>
		public override SCG.IEnumerator<T> GetEnumerator() {
			yield break;
		}
		/// <inheritdoc/>
		public override Result<TResult> OfType<TResult>(S.Func<T, S.Exception> onError) => Value;
		/// <inheritdoc/>
		public override string ToString() => $"Error<{typeof(T)}>({Value})";
		/// <inheritdoc/>
		public override STT.Task<Result<TResult>> TraverseAsync<TResult>(S.Func<T, STT.Task<TResult>> map)
		=> STT.Task.FromResult<Result<TResult>>(Value);
	}
}
