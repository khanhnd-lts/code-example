<template>
  <div class="header-container" :style="{background: colors.header.bg}">
    <slot
      name="header"
      :colors="colors"
      :chatTitle="chatTitle"
      :participants="participants"
      :myself="myself"
    ></slot>
    <div v-if="!hasHeaderSlot" class="header-title">
      <p
        class="header-title-text"
        v-if="chatTitle"
        :style="{color: colors.header.text}"
      >{{chatTitle}}</p>
      <p class="mb-0">
        <!--<span>{{myself.name}}, </span>-->
        <span v-for="(participant) in participants" :key="participant.id" class="mr-1">
          <router-link
            class="text-decoration-none header-paticipants-text"
            style="text-decoration:none;"
            :to="`/profile/${participant.account}`"
            v-if="participant.account !== myself.account"
          >{{participant.name}}</router-link>
        </span>
      </p>
    </div>

    <div v-if="!hideCloseButton && !hasHeaderSlot" class="header-exit">
      <slot name="close-button">
        <a
          class="header-exit-button d-flex align-center"
          href="#"
          :style="{fontSize: closeButtonIconSize}"
          @click.prevent="onClose"
        ><v-icon light size="20">close</v-icon></a>
      </slot>
    </div>
  </div>
</template>

<script lang="ts">
/*
 * TODO: improve support for more than 10 participants (the names list may break in some cases)
 */
import { Vue } from "vue-property-decorator";
export default Vue.extend({
  props: {
    colors: {
      type: Object,
      required: true,
    },
    borderStyle: {
      type: Object,
      required: false,
      default: () => {
        return {
          topLeft: "10px",
          topRight: "10px",
          bottomLeft: "10px",
          bottomRight: "10px",
        };
      },
    },
    hideCloseButton: {
      type: Boolean,
      required: false,
      default: false,
    },
    closeButtonIconSize: {
      type: String,
      required: false,
      default: "15px",
    },
    chatTitle: {
      type: String,
      required: false,
      default: "",
    },
    myself: {
      type: Object,
      required: false,
      default: {},
    },
    participants: {
      type: Array,
      required: false,
      default: [],
    },
  },
  computed: {
    hasHeaderSlot(): any {
      return !!this.$slots["header"];
    },
  },
  methods: {
    onClose: function () {
      this.$emit("onClose");
    },
  },
});
</script>

<style>
.quick-chat-container .header-container {
  height: 48px;
  display: flex;
  padding: 0 10px 0 10px;
  align-items: center;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1), 
              0 -1px rgba(0, 0, 0, 0.1) inset,
              0 2px 1px -1px rgba(255, 255, 255, 0.5) inset;
  z-index: 5;
}

.quick-chat-container .header-container .header-title {
  flex: 1;
  text-align: left;
}

.quick-chat-container .header-container .header-title-text {
  margin-bottom: 0;
}

.quick-chat-container .header-container .header-paticipants-text {
  color: #050505 !important;
  font-size: 16px;
  font-weight: 500;
  margin-top: 5px;
  max-height: 30px;
  overflow: hidden;
  margin-bottom: 5px;
}

.quick-chat-container .header-container .header-exit-button {
  text-decoration: none;
  color: #fff;
  font-size: 20px;
}

.quick-chat-container .header-container .icon-close-chat {
  color: #fff;
  width: 100%;
}

.quick-chat-container .header-container .icon-close-chat:hover {
  color: rgb(238, 121, 121);
}
</style>
