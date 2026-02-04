import { css, customElement, html } from "@umbraco-cms/backoffice/external/lit";
import {
  TranslatorAIConfigElement,
  TranslatorAIConfigElementBase,
} from "@jumoo/translate-ai";
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event";

@customElement("jumoo-tm-umbraco-ai-config")
export class JumooTmUmbracoAiConfigElement
  extends TranslatorAIConfigElementBase
  implements TranslatorAIConfigElement
{
  #onProfileChange(event: UmbChangeEvent) {
    event.stopPropagation();
    const picker = event.target as HTMLElement & { value: string | undefined };
    const profileId = picker.value ?? null;

    const addtionalValue = {
      ...this.settings?.additional,
      ...{ ["umbai-profileId"]: profileId },
    };

    this.dispatchEvent(
      new CustomEvent("ai-translator-config-update", {
        bubbles: true,
        composed: true,
        detail: { name: "additional", value: addtionalValue },
      }),
    );
  }

  override render() {
    return html`<h3>
        <umb-localize key="jumooAi_configHeading"></umb-localize>
      </h3>
      <div>${this._renderProfilePicker()}</div>
      <div class="info-box">
        <umb-localize key="jumooAi_configInstructions"
          >The Umbraco AI connector uses the Umbraco AI settings while
          translating content - setup your API connections via the API section
          in Umbraco.</umb-localize
        >
      </div>`;
  }

  _renderProfilePicker() {
    return html`<umb-property-layout
      .label=${this.localize.term("jumooAi_profile")}
      .description=${this.localize.term("jumooAi_description")}
    >
      <uai-profile-picker
        slot="editor"
        .value=${this._valueOrDefault("umbai-profileId", "") ?? undefined}
        @change=${this.#onProfileChange}
      ></uai-profile-picker>
    </umb-property-layout> `;
  }

  static override styles = [
    css`
      .info-box {
        margin-top: 10px;
        text-align: center;
        border: 1px solid var(--uui-color-border);
        border-radius: 4px;
        padding: 10px;
        background-color: var(--uui-color-default-emphasis);
        color: var(--uui-color-default-contrast);
      }
    `,
  ];
}

export default JumooTmUmbracoAiConfigElement;
