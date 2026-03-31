using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace LightingEssentials;

internal static class LightingSettingsDefaults
{
    private static readonly FieldInfo[] PublicSettingsFields = typeof(LightingSettings).GetFields(BindingFlags.Instance | BindingFlags.Public);

    /// <summary>
    /// Creates a new <see cref="LightingSettings"/> instance and fills every field with its declared default value.
    /// </summary>
    public static LightingSettings CreateDefaults()
    {
        LightingSettings defaults = new();
        ApplyDefaults(defaults);
        return defaults;
    }

    /// <summary>
    /// Applies all <see cref="DefaultValueAttribute"/> values from <see cref="LightingSettings"/> onto the target instance.
    /// </summary>
    /// <param name="target">The settings object to mutate.</param>
    public static void ApplyDefaults(LightingSettings target)
    {
        // Walk all public fields so this helper stays in sync as settings are added/removed.
        for (int i = 0; i < PublicSettingsFields.Length; i++)
        {
            FieldInfo field = PublicSettingsFields[i];
            if (!TryGetDefaultValue(field, out object value))
                continue;

            field.SetValue(target, value);
        }
    }

    /// <summary>
    /// Reads and converts a field's <see cref="DefaultValueAttribute"/> value to the field's concrete type.
    /// </summary>
    /// <param name="field">The reflected field whose default should be extracted.</param>
    /// <param name="value">The converted default value when available.</param>
    /// <returns><c>true</c> when a default value was found and converted; otherwise <c>false</c>.</returns>
    private static bool TryGetDefaultValue(FieldInfo field, out object value)
    {
        DefaultValueAttribute defaultValue = field.GetCustomAttribute<DefaultValueAttribute>();
        if (defaultValue is null)
        {
            value = null;
            return false;
        }

        object rawValue = defaultValue.Value;
        if (rawValue is null)
        {
            value = null;
            return false;
        }

        if (field.FieldType.IsInstanceOfType(rawValue))
        {
            value = rawValue;
            return true;
        }

        if (rawValue is string stringValue)
        {
            // Color and other non-primitive defaults are often represented as strings in attributes.
            TypeConverter converter = TypeDescriptor.GetConverter(field.FieldType);
            if (converter.CanConvertFrom(typeof(string)))
            {
                object converted = converter.ConvertFromInvariantString(stringValue);
                if (converted is not null)
                {
                    value = converted;
                    return true;
                }
            }
        }

        try
        {
            value = Convert.ChangeType(rawValue, field.FieldType, CultureInfo.InvariantCulture);
            return true;
        }
        catch
        {
            value = null;
            return false;
        }
    }
}
