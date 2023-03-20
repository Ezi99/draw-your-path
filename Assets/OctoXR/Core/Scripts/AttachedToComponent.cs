using System;
using UnityEngine;

namespace OctoXR
{
    /// <summary>
    /// Base class for a behaviour that requires always being attached to a specific other component. It is assumed that the component
    /// the behaviour is attached to also has a reference or some kind of mechanism that provides it with knowledge of the behaviour being
    /// attached to it. This script executes in the edit mode
    /// </summary>
    /// <typeparam name="TComponent">The type of component this behaviour is always linked to</typeparam>
    [ExecuteAlways]
    public abstract class AttachedToComponent<TComponent> : MonoBehaviour where TComponent : Component
	{
		[SerializeField]
        [HideInInspector]
        private TComponent componentAttachedTo;
		/// <summary>
		/// The component the behaviour is linked to
		/// </summary>
		protected TComponent ComponentAttachedTo => componentAttachedTo;

		/// <summary>
		/// Indicates whether the behaviour is attached to a component
		/// </summary>
		protected bool IsAttached => componentAttachedTo;

		[SerializeField]
		[HideInInspector]
		private new Transform transform;
		/// <summary>
		/// The tranform of the behaviour object
		/// </summary>
		public Transform Transform => transform;

		[SerializeField]
		[HideInInspector]
		private bool notifiedAttachedToComponent;

		protected virtual void Reset()
		{
			if (!transform)
			{
				transform = base.transform;
				notifiedAttachedToComponent = TryFindComponentAcknowledgingBehaviourAttachedToIt(true, out componentAttachedTo);
			}
		}

		protected virtual void Awake()
		{
			InitializeAndEnsureBehaviourStateIsValid();
		}

		protected virtual void OnValidate()
		{
			InitializeAndEnsureBehaviourStateIsValid();
		}

		protected virtual void OnDestroy()
		{
			componentAttachedTo = null;
		}

		private void InitializeAndEnsureBehaviourStateIsValid()
		{
			transform = base.transform;

			if (componentAttachedTo && !IsBehaviourAttachmentAcknowledgedByComponent(componentAttachedTo))
			{
				if (TryFindComponentAcknowledgingBehaviourAttachedToIt(false, out var componentAttachedTo))
				{
					this.componentAttachedTo = componentAttachedTo;
					notifiedAttachedToComponent = true;
				}
				else
				{
					var component = this.componentAttachedTo;

					this.componentAttachedTo = null;
					notifiedAttachedToComponent = false;

					ObjectUtility.SetObjectDirty(this);

					OnAttachmentToComponentNotAcknowledged(component);
				}
			}
		}

		/// <summary>
		/// Assigns the specified component to the behaviour. Note that this method is intended to be used by a specific component when
		/// it wants to attach the behaviour to itself, calling it from any other context is likely going to throw an exception
		/// </summary>
		/// <param name="component"></param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="ArgumentException"></exception>
		public void AttachToComponent(TComponent component)
		{
			if (!component)
			{
				throw new ArgumentNullException(nameof(component));
			}

			if (componentAttachedTo)
			{
				throw new InvalidOperationException($"{GetType().Name} is already attached to a {typeof(TComponent).Name}");
			}

			if (!IsBehaviourAttachmentAcknowledgedByComponent(component))
			{
				throw new ArgumentException($"{GetType().Name} has not been attached from the specified {typeof(TComponent).Name}");
			}

			OnAttachingToComponent(component);

			componentAttachedTo = component;

			if (!notifiedAttachedToComponent)
			{
				notifiedAttachedToComponent = true;
				OnAttachedToComponent();
#if UNITY_EDITOR
				if (notifiedAttachedToComponent)
				{
					ObjectUtility.SetObjectDirty(this);
				}
#endif
			}
		}

		/// <summary>
		/// Detaches the behaviour from the current component it is attached to. Note that this method is intended to be used by 
		/// a specific component when it wants to detach the behaviour from itself, calling it from any other context is likely 
		/// going to throw an exception
		/// </summary>
		/// <param name="component"></param>
		/// <exception cref="InvalidOperationException"></exception>
		public void DetachFromComponent()
		{
			if (!componentAttachedTo)
			{
				throw new InvalidOperationException($"{GetType().Name} is not attached to a {typeof(TComponent).Name}");
			}

			if (IsBehaviourAttachmentAcknowledgedByComponent(componentAttachedTo))
			{
				throw new InvalidOperationException($"{GetType().Name} has not been detached from the context of its current " +
					$"{typeof(TComponent).Name}");
			}

			OnDetachingFromComponent();

			var component = componentAttachedTo;

			componentAttachedTo = null;

			if (notifiedAttachedToComponent)
			{
				notifiedAttachedToComponent = false;
				OnDetachedFromComponent(component);
#if UNITY_EDITOR
				if (!notifiedAttachedToComponent)
				{
					ObjectUtility.SetObjectDirty(this);
				}
#endif
			}
		}

		/// <summary>
		/// Called when the behaviour is linked to a component. Used as callback only
		/// </summary>
		protected virtual void OnAttachedToComponent() { }

		/// <summary>
		/// Called when the behaviour is unlinked from the specified component. Used as callback only
		/// </summary>
		/// <param name="component"></param>
		protected virtual void OnDetachedFromComponent(TComponent component) { }

		/// <summary>
		/// Called when the behaviour is getting linked to the specified component just before any values are assigned. Used as callback only
		/// </summary>
		/// <param name="component"></param>
		protected virtual void OnAttachingToComponent(TComponent component) { }

		/// <summary>
		/// Called when the behaviour is getting unlinked from the component it is attached to. Used as callback only
		/// </summary>
		protected virtual void OnDetachingFromComponent() { }

		/// <summary>
		/// Callback sent in certain circumstances when the behaviour detects the component it considers itself attached to does not
		/// acknowledge the behaviour being attached to it. This scenario gets checked for in Awake and OnValidate methods and it usually 
		/// happens as a result of calling Object.Instantiate or copy-pasting the behaviour in Unity editor. Before this gets called the 
		/// value of <see cref="ComponentAttachedTo"/> is cleared. Note that under this circumstance OnDetachedFromComponent callback is 
		/// not sent
		/// </summary>
		/// <param name="component"></param>
		protected virtual void OnAttachmentToComponentNotAcknowledged(TComponent component) { }

		/// <summary>
		/// Indicates whether the specified component is in some way aware of the behaviour being attached to it regardless of the specified 
		/// component not yet being acknowledged by the behaviour
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		protected abstract bool IsBehaviourAttachmentAcknowledgedByComponent(TComponent component);

		/// <summary>
		/// Tries to find a component that is aware of the behaviour being attached to it regardless of the specified component not yet 
		/// being acknowledged by the behaviour. This method gets called when the behaviour is reset in editor and is part of behaviour 
		/// state reconstruction after every such occurrence. It can also be potentially called from Awake and OnValidate. From which 
		/// context the call is made is indicated by <paramref name="isReset"/> parameter
		/// </summary>
		/// <param name="isReset"></param>
		/// <param name="componentAttachedTo"></param>
		/// <returns></returns>
		protected abstract bool TryFindComponentAcknowledgingBehaviourAttachedToIt(bool isReset, out TComponent componentAttachedTo);
	}
}
