using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomUIDemo
{
    class EditorTextChangedBehavior : Behavior<CustomStyleEditor>
    {
        protected override void OnAttachedTo(CustomStyleEditor bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += OnEditorTextChanged;
        }

        protected override void OnDetachingFrom(CustomStyleEditor bindable)
        {
            bindable.TextChanged -= OnEditorTextChanged;
            base.OnDetachingFrom(bindable);
        }

        private void OnEditorTextChanged(object? sender, TextChangedEventArgs e)
        {
            var viewModel = (sender as CustomStyleEditor)!.BindingContext as ViewModel;

            // Disable send icon when Stop responding is loading.
            viewModel!.EnableSendIcon = !string.IsNullOrEmpty(e.NewTextValue);
        }
    }
}
