// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

(() => {
	const debounce = (fn, delay = 250) => {
		let timerId;
		return (...args) => {
			window.clearTimeout(timerId);
			timerId = window.setTimeout(() => fn(...args), delay);
		};
	};

	const formatDateTime = (date) => {
		const locale = navigator.language || "en-US";
		return new Intl.DateTimeFormat(locale, {
			year: "numeric",
			month: "2-digit",
			day: "2-digit",
			hour: "2-digit",
			minute: "2-digit"
		}).format(date);
	};

	const getLocaleConfig = () => {
		const locale = (navigator.language || "en-US").toLowerCase();
		const isHr = locale.startsWith("hr");
		return {
			isHr,
			dateFormat: isHr ? "d.m.Y H:i" : "m/d/Y h:i K",
			time_24hr: isHr,
			locale: isHr && window.flatpickr?.l10ns?.hr ? window.flatpickr.l10ns.hr : undefined
		};
	};

	const parseInputWithFormat = (value, format) => {
		if (!value || !window.flatpickr) return null;
		const parsed = window.flatpickr.parseDate(value, format);
		if (parsed && !Number.isNaN(parsed.getTime())) {
			return parsed;
		}
		const fallback = new Date(value);
		return Number.isNaN(fallback.getTime()) ? null : fallback;
	};

	const toDateInputValue = (date) => {
		const year = date.getFullYear();
		const month = String(date.getMonth() + 1).padStart(2, "0");
		const day = String(date.getDate()).padStart(2, "0");
		return `${year}-${month}-${day}`;
	};

	const toTimeInputValue = (date) => {
		const hours = String(date.getHours()).padStart(2, "0");
		const minutes = String(date.getMinutes()).padStart(2, "0");
		return `${hours}:${minutes}`;
	};

	const parseInputToDate = (value) => {
		if (!value) return null;
		const parsed = new Date(value);
		if (!Number.isNaN(parsed.getTime())) {
			return parsed;
		}
		const parts = value.match(/(\d{1,2})\D(\d{1,2})\D(\d{2,4})/);
		if (parts) {
			const day = parseInt(parts[1], 10);
			const month = parseInt(parts[2], 10) - 1;
			const year = parseInt(parts[3], 10);
			const timeMatch = value.match(/(\d{1,2}):(\d{2})/);
			const hours = timeMatch ? parseInt(timeMatch[1], 10) : 0;
			const minutes = timeMatch ? parseInt(timeMatch[2], 10) : 0;
			return new Date(year, month, day, hours, minutes);
		}
		return null;
	};

	const wireQuestionBump = () => {
		document.querySelectorAll(".js-question-bump").forEach((button) => {
			button.addEventListener("click", () => {
				button.classList.remove("is-bumped");
				window.requestAnimationFrame(() => button.classList.add("is-bumped"));
			});
		});
	};

	const wireAlerts = () => {
		document.querySelectorAll(".mario-alert, .login-alert").forEach((alert) => {
			window.requestAnimationFrame(() => alert.classList.add("is-visible"));
			window.setTimeout(() => alert.classList.add("is-fading"), 3500);
			window.setTimeout(() => alert.remove(), 4200);
		});
	};

	const wireDateTimePicker = () => {
		if (!window.flatpickr) return;

		const closeAll = (except) => {
			document.querySelectorAll(".mario-datetime").forEach((picker) => {
				if (picker !== except) {
					picker.classList.remove("is-open");
					const popup = picker.querySelector(".mario-datetime-popup");
					popup?.setAttribute("aria-hidden", "true");
				}
			});
		};

		document.addEventListener("click", (event) => {
			const picker = event.target.closest(".mario-datetime");
			closeAll(picker);
		});

		document.querySelectorAll(".mario-datetime").forEach((wrapper) => {
			const input = wrapper.querySelector(".js-datetime-input");
			const popup = wrapper.querySelector(".mario-datetime-popup");
			const calendar = wrapper.querySelector(".js-datetime-calendar");
			const nowButtons = wrapper.querySelectorAll(".js-datetime-now");
			const clearButtons = wrapper.querySelectorAll(".js-datetime-clear");
			const applyButton = wrapper.querySelector(".js-datetime-apply");
			const closeButton = wrapper.querySelector(".js-datetime-close");

			if (!input || !popup || !calendar) return;

			const { dateFormat, time_24hr, locale } = getLocaleConfig();
			const defaultDate = parseInputWithFormat(input.value.trim(), dateFormat) || new Date();
			const picker = window.flatpickr(calendar, {
				inline: true,
				enableTime: true,
				time_24hr,
				dateFormat,
				defaultDate,
				locale
			});

			const syncFromInput = () => {
				const parsed = parseInputWithFormat(input.value.trim(), dateFormat);
				if (parsed) {
					picker.setDate(parsed, false);
				}
			};

			const applyToInput = () => {
				const date = picker.selectedDates[0];
				if (!date) {
					input.value = "";
					input.dispatchEvent(new Event("change"));
					return;
				}
				input.value = picker.formatDate(date, dateFormat);
				input.dispatchEvent(new Event("change"));
			};

			const positionPopup = () => {
				const rect = input.getBoundingClientRect();
				const popupRect = popup.getBoundingClientRect();
				const padding = 8;
				let left = rect.left;
				const maxLeft = window.innerWidth - popupRect.width - padding;
				if (left > maxLeft) {
					left = Math.max(padding, maxLeft);
				}
				let top = rect.bottom + padding;
				const maxTop = window.innerHeight - popupRect.height - padding;
				if (top > maxTop) {
					top = Math.max(padding, rect.top - popupRect.height - padding);
				}
				popup.style.left = `${Math.max(padding, left)}px`;
				popup.style.top = `${Math.max(padding, top)}px`;
			};

			input.addEventListener("focus", () => {
				closeAll(wrapper);
				syncFromInput();
				wrapper.classList.add("is-open");
				popup.setAttribute("aria-hidden", "false");
				requestAnimationFrame(positionPopup);
			});

			input.addEventListener("click", () => {
				closeAll(wrapper);
				syncFromInput();
				wrapper.classList.add("is-open");
				popup.setAttribute("aria-hidden", "false");
				requestAnimationFrame(positionPopup);
			});

			nowButtons.forEach((button) => {
				button.addEventListener("click", (event) => {
					event.stopPropagation();
					picker.setDate(new Date(), false);
					applyToInput();
				});
			});

			clearButtons.forEach((button) => {
				button.addEventListener("click", (event) => {
					event.stopPropagation();
					picker.clear();
					input.value = "";
					input.dispatchEvent(new Event("change"));
				});
			});

			applyButton?.addEventListener("click", (event) => {
				event.stopPropagation();
				applyToInput();
				wrapper.classList.remove("is-open");
				popup.setAttribute("aria-hidden", "true");
			});

			closeButton?.addEventListener("click", (event) => {
				event.stopPropagation();
				wrapper.classList.remove("is-open");
				popup.setAttribute("aria-hidden", "true");
			});

			popup.addEventListener("click", (event) => {
				event.stopPropagation();
			});
		});
	};

	const wireAutocomplete = () => {
		const closeAll = (except) => {
			document.querySelectorAll(".mario-autocomplete").forEach((wrapper) => {
				if (wrapper !== except) {
					wrapper.classList.remove("is-open");
				}
			});
		};

		document.addEventListener("click", (event) => {
			const wrapper = event.target.closest(".mario-autocomplete");
			closeAll(wrapper);
		});

		document.querySelectorAll(".mario-autocomplete").forEach((wrapper) => {
			const input = wrapper.querySelector(".js-autocomplete-input");
			const hidden = wrapper.querySelector(".js-autocomplete-hidden");
			const results = wrapper.querySelector(".mario-autocomplete-results");
			const url = wrapper.dataset.searchUrl;

			if (!input || !results || !url) return;

			const renderResults = (items) => {
				results.innerHTML = "";
				if (!items.length) {
					results.innerHTML = '<div class="mario-autocomplete-empty">No matches</div>';
					return;
				}

				items.forEach((item) => {
					const button = document.createElement("button");
					button.type = "button";
					button.className = "mario-autocomplete-item";
					button.textContent = item.text;
					button.dataset.id = item.id;
					results.appendChild(button);
				});
			};

			const fetchResults = debounce(async () => {
				const term = input.value.trim();
				if (!term) {
					results.innerHTML = "";
					wrapper.classList.remove("is-open");
					hidden.value = "";
					return;
				}

				wrapper.classList.add("is-loading");
				try {
					const response = await fetch(`${url}?term=${encodeURIComponent(term)}`);
					const data = await response.json();
					renderResults(data);
					wrapper.classList.add("is-open");
				} catch {
					results.innerHTML = '<div class="mario-autocomplete-empty">Error loading results</div>';
					wrapper.classList.add("is-open");
				} finally {
					wrapper.classList.remove("is-loading");
				}
			}, 250);

			input.addEventListener("input", () => {
				hidden.value = "";
				fetchResults();
			});

			results.addEventListener("click", (event) => {
				const item = event.target.closest(".mario-autocomplete-item");
				if (!item) return;
				input.value = item.textContent || "";
				hidden.value = item.dataset.id || "";
				wrapper.classList.remove("is-open");
			});
		});
	};

	const wireEntrySearch = () => {
		const input = document.querySelector(".js-entry-search");
		const list = document.querySelector("#game-entry-list");
		if (!input || !list) return;

		const url = input.dataset.searchUrl;
		const spinner = input.parentElement?.querySelector(".mario-search-spinner");
		if (!url) return;

		const performSearch = debounce(async () => {
			const term = input.value.trim();
			list.classList.add("is-loading");
			spinner?.classList.add("is-loading");
			try {
				const response = await fetch(`${url}?term=${encodeURIComponent(term)}`);
				const html = await response.text();
				list.innerHTML = html;
				list.classList.add("is-updated");
				window.setTimeout(() => list.classList.remove("is-updated"), 250);
			} catch {
				list.innerHTML = '<div class="mario-empty">Search failed. Try again.</div>';
			} finally {
				list.classList.remove("is-loading");
				spinner?.classList.remove("is-loading");
			}
		}, 300);

		input.addEventListener("input", performSearch);
	};

	const wireListSearch = () => {
		document.querySelectorAll(".js-list-search").forEach((input) => {
			const target = input.dataset.target;
			const url = input.dataset.searchUrl;
			if (!target || !url) return;

			const list = document.querySelector(target);
			if (!list) return;

			const spinner = input.parentElement?.querySelector(".mario-search-spinner");
			const performSearch = debounce(async () => {
				const term = input.value.trim();
				list.classList.add("is-loading");
				spinner?.classList.add("is-loading");
				try {
					const response = await fetch(`${url}?term=${encodeURIComponent(term)}`);
					const html = await response.text();
					list.innerHTML = html;
					list.classList.add("is-updated");
					window.setTimeout(() => list.classList.remove("is-updated"), 250);
				} catch {
					list.innerHTML = '<div class="mario-empty">Search failed. Try again.</div>';
				} finally {
					list.classList.remove("is-loading");
					spinner?.classList.remove("is-loading");
				}
			}, 300);

			input.addEventListener("input", performSearch);
		});
	};

	const wireValidation = () => {
		if (window.jQuery && window.jQuery.validator) {
			window.jQuery.validator.setDefaults({
				onfocusout(element) {
					this.element(element);
				},
				ignore: ":hidden:not(.js-autocomplete-hidden)"
			});
		}
	};

	document.addEventListener("DOMContentLoaded", () => {
		wireQuestionBump();
		wireAlerts();
		wireDateTimePicker();
		wireAutocomplete();
		wireEntrySearch();
		wireListSearch();
		wireValidation();
	});
})();
