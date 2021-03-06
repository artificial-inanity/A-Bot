﻿using BotEngine.Motor;
using Sanderling.Accumulation;
using Sanderling.Motor;
using System.Collections.Generic;
using System.Linq;
using WindowsInput.Native;

namespace Sanderling.ABot.Bot.Task
{
	static public class ModuleTaskExtension
	{
		static public bool? IsActive(
			this IShipUiModule module,
			Bot bot)
		{
			if (bot?.MouseClickLastAgeStepCountFromUIElement(module) <= 1)
				return null;

			if (bot?.ToggleLastAgeStepCountFromModule(module) <= 1)
				return null;

			return module?.RampActive;
		}

		static public IBotTask EnsureIsActive(
			this Bot bot,
			IShipUiModule module)
		{
			if (module?.IsActive(bot) ?? true)
				return null;

			return new ModuleToggleTask { bot = bot, module = module };
		}

		public static IBotTask DeactiveModule(
			this Bot bot,
			IShipUiModule module)
		{
			if (module?.IsActive(bot) == false || module?.RampActive == false)
			{
				return null;
			}
			else
			{
				return new ModuleToggleTask { bot = bot, module = module };
			}
		}



		static public IBotTask EnsureIsActive(
			this Bot bot,
			IEnumerable<IShipUiModule> setModule) =>
			new BotTask { Component = setModule?.Select(module => bot?.EnsureIsActive(module)) };

		public static IBotTask DeactivateModule(
			this Bot bot,
			IEnumerable<IShipUiModule> setModule) =>
			new BotTask { Component = setModule?.Select(module => bot?.DeactiveModule(module)) };
	}

	public class ModuleToggleTask : IBotTask
	{
		public Bot bot;

		public IShipUiModule module;

		public IEnumerable<IBotTask> Component => null;

		public IEnumerable<MotionParam> Effects
		{
			get
			{
				var toggleKey = module?.TooltipLast?.Value?.ToggleKey;

				if (0 < toggleKey?.Length)
					yield return toggleKey?.KeyboardPressCombined();

				yield return module?.MouseClick(MouseButtonIdEnum.Left);
			}
		}

		public IBotTask ReloadAnomaly()
		{
			var ReloadAnomalyFactory = new BotTask { Component = null, Effects = ReloadAnomalyFunction() };
			return ReloadAnomalyFactory;
		}


		public IEnumerable<MotionParam> ReloadAnomalyFunction()
		{
			var APPS = VirtualKeyCode.APPS;

			yield return APPS.KeyboardPress();
			yield return APPS.KeyboardPress();
		}
	}
}